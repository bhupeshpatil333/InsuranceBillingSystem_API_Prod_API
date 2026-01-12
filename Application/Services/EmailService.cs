using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            byte[] attachment,
            string fileName)
        {
            var smtp = new SmtpClient
            {
                Host = _config["Email:SmtpServer"],
                Port = int.Parse(_config["Email:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _config["Email:Username"],
                    _config["Email:Password"]
                )
            };

            var mail = new MailMessage
            {
                From = new MailAddress(
                    _config["Email:From"]
                ),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            if (attachment != null)
            {
                mail.Attachments.Add(
                    new Attachment(
                        new MemoryStream(attachment),
                        fileName,
                        "application/pdf"
                    )
                );
            }

            await smtp.SendMailAsync(mail);
        }
    }
}
