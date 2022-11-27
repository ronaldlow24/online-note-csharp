
namespace OnlineNote.Common
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ConnectionStrings
    {
        public string Database { get; set; }
    }

    public static class ApplicationSetting
    {
        public static ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public static EmailConfiguration EmailConfiguration { get; set; } = new EmailConfiguration();
    }
}
