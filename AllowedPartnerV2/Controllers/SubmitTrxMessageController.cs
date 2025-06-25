using AllowedPartnerV2.Dto;
using AllowedPartnerV2.Model;
using AllowedPartnerV2.Output;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AllowedPartnerV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmitTrxMessageController : ControllerBase
    {
        private readonly Context _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(SubmitTrxMessageController));

        public SubmitTrxMessageController(Context context)
        {
            _context = context;
        }
       
        [HttpPost]
        public async Task<ActionResult<Response>> Get(Partner partner)
        {
            Response r = new Response();
            var pt = await _context.Partners
                .Include(c => c.Items)
                .Where(a => a.partnerkey == partner.partnerkey && a.sig == partner.sig)
                .Select(c => new PartnerDto
                {
                    partnerkey = c.partnerkey,
                    partnerrefno = c.partnerrefno,
                    partnerpassword = c.partnerpassword,
                    totalamount = c.totalamount,
                    items = c.Items.Select(p => new ItemDto
                    {
                        partneritemref = p.partneritemref,
                        name = p.name,
                        qty = p.qty,
                        unitprice = p.unitprice
                    }).ToList(),
                    timestamp = c.timestamp,
                    sig = c.sig
                })
                .SingleOrDefaultAsync();

            if (!ModelState.IsValid)
            {
                var missingFields = ModelState
                                    .Where(kvp => kvp.Value.Errors.Count > 0)
                                    .Select(kvp => new {
                                        Field = kvp.Key,
                                        Errors = kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                    });
                r.result = 0;
                r.resultmessage = missingFields.ToString();
                log.Error(r);
                return r;
            }


            if (pt == null)
            {
                //return StatusCode((int)HttpStatusCode.Unauthorized, "Access Denied!");
              

                r.result = 0;
                r.resultmessage = "Access Denied!";
                log.Error(r);
                return r;
            }

            if(partner.Items.Count > 0)
            {
                long actualtotal = 0;

                foreach (var item in partner.Items)
                {
                    actualtotal = actualtotal + (item.qty * item.unitprice);
                }

                if (actualtotal != pt.totalamount)
                {
                    

                    r.result = 0;
                    r.resultmessage = "Invalid Total Amount.";
                    log.Error(r);
                    return r;
                }

                DateTime currentDt = DateTime.Parse(partner.timestamp);

                DateTime setDt = DateTime.Parse(pt.timestamp);

                TimeSpan difference = setDt - currentDt;

                if(Math.Abs(difference.TotalMinutes) > 5)
                {
                    r.result = 0;
                    r.resultmessage = "Expired.";
                    log.Error(r);
                    return r;
                }
            }

           
            r.result = 1;
            r.totalamount = pt.totalamount;
            r.totaldiscount = GetDiscount(pt.totalamount);
            r.finalamount = r.totalamount - r.totaldiscount;
            log.Info(r);
            return r;
        }

        private static bool IsPrimeNumber(long num)
        {
            if (num <= 1)
                return false;
            if (num == 2)
                return true;
            if (num % 2 == 0)
                return false;

            long limit = (long)Math.Sqrt(num);

            for (int i = 3; i <= limit; i += 2)
            {
                if (num % i == 0)
                    return false;
            }

            return true;
        }

        private long GetDiscount(long total)
        {
            long finalamt = 0;
            long totaldiscount = 0;

            if (total >= 200 && total <= 500)
                return (long)(0.05 * total);
            else if (total >= 501 && total <= 800)
            {
                finalamt = (long)(0.07 * total);
                totaldiscount = (long)0.07;
            }
            else if (total >= 801 && total <= 1200)
            {
                finalamt = (long)(0.1 * total);
                totaldiscount = (long)0.1;
            }
            else if (total > 1200)
            {
                finalamt = (long)(0.15 * total);
                totaldiscount = (long)0.15;
            }

            if(totaldiscount == (long)0.15)
            {
                return finalamt;
            }
            else
            {
                if (total.ToString()[total.ToString().Length - 1] == '5' && total > 900 && totaldiscount > 0)
                {
                    return finalamt + (long)(total * 0.1);
                }
                else if(IsPrimeNumber(total) && total > 500 && totaldiscount > 0)
                {
                    return finalamt + (long)(total * 0.08);
                }
            }

            return 0;
        }
    }
}
