using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AllowedPartnerV2.Model
{
    public class Items
    {
        [Key]
        [Required]
        public string partneritemref { get; set; } = String.Empty;

        [Required]
        public string name { get; set; } = String.Empty;

        [Required]
        public int qty { get; set; }

        [Required]
        public long unitprice { get; set; }

     
        public Partner? Partner
        {
            get;
            
        }
    }
}
