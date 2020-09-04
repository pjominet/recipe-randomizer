using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Utils.Settings;

namespace RecipeRandomizer.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string html, string sender = null)
        {
            try
            {
                // create message
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(sender ?? _emailSettings.Sender)
                };
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, sender ?? _emailSettings.Sender));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = html
                };

                // send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpKey);
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
