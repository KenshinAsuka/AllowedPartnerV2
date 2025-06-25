namespace AllowedPartnerV2.Output
{
    public class Response
    {
        public int result {  get; set; }

        public long totalamount { get; set; }

        public long totaldiscount { get; set; }

        public long finalamount { get; set; }

        public string resultmessage { get; set; } = String.Empty;

    }
}
