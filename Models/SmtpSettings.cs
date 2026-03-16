namespace KiefProductions_Site.Models
{
    public class SmtpSettings
    {
        public string Host { get; set; }          // SMTP server host
        public int Port { get; set; }             // SMTP port
        public string Username { get; set; }      // SMTP username (email)
        public string Password { get; set; }      // SMTP password
        public bool UseSsl { get; set; }          // SSL/TLS flag
        public string RecipientEmail { get; set; } // Where to send the contact form emails
        public string RecipientName { get; set; }  // Display name for recipient
    }
}
