namespace AllowedPartnerV2.Dto
{
    public class PartnerDto
    {
        public string partnerkey { get; set; } = String.Empty;

        public string partnerrefno { get; set; } = String.Empty;

        public string partnerpassword { get; set; } = String.Empty;

        public long totalamount { get; set; }

        public List<ItemDto> items { get; set; }

        public string timestamp { get; set; } = String.Empty;

        public string sig { get; set; } = String.Empty;
    }
}
