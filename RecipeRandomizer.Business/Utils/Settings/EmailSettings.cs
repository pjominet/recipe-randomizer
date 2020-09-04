namespace RecipeRandomizer.Business.Utils.Settings
{
    public class EmailSettings
    {
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpKey { get; set; }
    }
}
