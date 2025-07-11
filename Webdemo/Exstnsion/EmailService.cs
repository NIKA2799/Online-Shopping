using Interface;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Interface.Model;

namespace Webdemo.Exstnsion
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;

    public class EmailService : IEmailConfiguration
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Sends an HTML email using SMTP.
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {ToEmail}", toEmail);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error while sending email to {ToEmail}", toEmail);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {ToEmail}", toEmail);
                throw;
            }
        }
    }
}