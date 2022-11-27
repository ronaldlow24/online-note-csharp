﻿using System.Net.Mail;
using System.Net;
using static OnlineNote.Common.Constant;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace OnlineNote.Common
{
    public class Mail
    {

        private static readonly SmtpClient smtpClient = new SmtpClient(ApplicationSetting.EmailConfiguration.SmtpServer)
        {
            Port = ApplicationSetting.EmailConfiguration.Port,
            Credentials = new NetworkCredential(ApplicationSetting.EmailConfiguration.UserName, ApplicationSetting.EmailConfiguration.Password),
            EnableSsl = true,
        };

        public static async Task SendMail(string subject, string message, IEnumerable<string> recipients)
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

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch
            {
                throw;
            }
        }
    }
}