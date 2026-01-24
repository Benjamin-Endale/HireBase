using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace HireBase_Backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // Generic email sender
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is null or empty", nameof(toEmail));

            var emailSettings = _config.GetSection("EmailSettings");

            var smtpServer = emailSettings["SmtpServer"];
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];
            var password = emailSettings["Password"];
            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(password))
                throw new Exception("Email settings are missing in configuration.");

            if (!int.TryParse(emailSettings["Port"], out int port))
                throw new Exception("SMTP Port is missing or invalid.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName!, senderEmail!));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(smtpServer!, port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(senderEmail!, password!);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // Optional: Keep OTP method if needed
        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var message = $"Applicant: {otp}. It expires in 5 minutes.";
            await SendEmailAsync(toEmail, "Your OTP Code", message);
        }
    }
}
