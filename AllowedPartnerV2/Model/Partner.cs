using System.ComponentModel.DataAnnotations;

namespace AllowedPartnerV2.Model
{
    public class Partner
    {
        [Key]
        [Required]
        public string partnerkey { get; set; } = String.Empty;

        [Required]
        public string partnerrefno { get; set; } = String.Empty;

        [Required]
        public string partnerpassword { get; set; } = String.Empty;

        [Required]
        public long totalamount { get; set; }

        public virtual ICollection<Items> Items        {
            get;
            set;
        } = new List<Items>();

        [Required]
        public string timestamp { get; set; } = String.Empty;

        public string sig { get; set; } = String.Empty;


    }
}
