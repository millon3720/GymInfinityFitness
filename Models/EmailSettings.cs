namespace Tesina.Models
{
    public class EmailSettings
    {
        public string Host { get; set; } = Environment.GetEnvironmentVariable("SMTP_HOST");
        public int Port { get; set; } = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var p) ? p : 587;
        public string User { get; set; } = Environment.GetEnvironmentVariable("SMTP_USER");
        public string Password { get; set; } = Environment.GetEnvironmentVariable("SMTP_PASS");
    }


}
