﻿using System.Net.Mail;
using System.Net;
using static OnlineNote.Common.Constant;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OnlineNote.Common
{
    public class MailHelper
    {

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static async Task SendMailAsync(string subject, string message, string recipient, CancellationToken cancellationToken = default)
        {
            await SendMailAsync(subject,message,new List<string> { recipient },cancellationToken);
        }
    
        public static async Task SendMailAsync(string subject, string message, IEnumerable<string> recipients, CancellationToken cancellationToken = default)
        {
            try
            {
                using var smtpClient = new SmtpClient(ApplicationSetting.EmailConfiguration.SmtpServer)
                {
                    Port = ApplicationSetting.EmailConfiguration.Port,
                    Credentials = new NetworkCredential(ApplicationSetting.EmailConfiguration.UserName, ApplicationSetting.EmailConfiguration.Password),
                    EnableSsl = true,
                };

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
            catch (Exception e)
            {
                throw;
            }
        }

        public static async Task SendErrorMailAsync(Exception ex)
        {
            var targetList = new List<string>() { "ron556611@gmail.com" };
            var body = $@"


            ";

            await SendMailAsync("Online Note Error", body, targetList);
        }
    }
}
