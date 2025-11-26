namespace Tesina.Models
{
    public class EmailSettings
    {

        public string Host => Environment.GetEnvironmentVariable("SMTP_HOST");
        public int Port => int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        public string User => Environment.GetEnvironmentVariable("SMTP_USER");
        public string Password => Environment.GetEnvironmentVariable("SMTP_PASS");


    }

}
