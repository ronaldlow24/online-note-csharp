using System.Net.Mail;
using System.Net;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Common
{
    public class MailHelper
    {

        private static readonly SmtpClient smtpClient = new SmtpClient(ApplicationSetting.EmailConfiguration.SmtpServer)
        {
            Port = ApplicationSetting.EmailConfiguration.Port,
            Credentials = new NetworkCredential(ApplicationSetting.EmailConfiguration.UserName, ApplicationSetting.EmailConfiguration.Password),
            EnableSsl = true,
        };

        public static async Task SendMailAsync(string subject, string message, IEnumerable<string> recipients, CancellationToken cancellationToken = default)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(ApplicationSetting.EmailConfiguration.From),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                foreach (var item in recipients)
                {
                    mailMessage.To.Add(item);
                }

                await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        public static async Task SendMailAsync(string subject, string message, string recipient, CancellationToken cancellationToken = default)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(ApplicationSetting.EmailConfiguration.From),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(recipient);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch
            {
                throw;
            }
        }
    }
}
