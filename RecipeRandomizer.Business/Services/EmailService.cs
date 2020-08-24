using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using RecipeRandomizer.Business.Interfaces;

namespace RecipeRandomizer.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _from;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpKey;

        public EmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSetting");
            _from = emailSettings.GetValue<string>("From");
            _smtpHost = emailSettings.GetValue<string>("SmtpHost");
            _smtpPort = emailSettings.GetValue<int>("SmtpPort");
            _smtpUser = emailSettings.GetValue<string>("SmtpUser");
            _smtpKey = emailSettings.GetValue<string>("SmtpKey");
        }

        public async Task SendEmailAsync(string to, string subject, string html, string from = null)
        {
            try
            {
                // create message
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(from ?? _from)
                };
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = html
                };

                // send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUser, _smtpKey);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
