using CashFlow.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace CashFlow.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_configuration["EmailSettings:FromName"], _configuration["EmailSettings:FromAddress"]));

            email.To.Add(MailboxAddress.Parse(to));

            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:Host"],
                    int.Parse(_configuration["EmailSettings:Port"]!),
                    SecureSocketOptions.StartTls
                );
                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:UserName"],
                    _configuration["EmailSettings:Password"]
                );

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Nie uda³o siê wys³aæ maila: {ex.Message}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}