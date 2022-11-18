using Blog_MVC.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Blog_MVC.Services
{
    public class EmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // var emailSender = _mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");
            // gather configuration data
            string emailAddress = _mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress")!;
            string emailHost = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost")!;
            int emailPort = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
            string emailPassword = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword")!;

            MimeMessage newEmail = new()
            {
                Sender = MailboxAddress.Parse("ayangdevv@gmail.com")
            };

            if (subject.Contains("Contact Me:"))
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            } else
            {
                newEmail.To.Add(MailboxAddress.Parse(email));
            }

            newEmail.Subject = subject;

            BodyBuilder emailBody = new();

            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();
            
            using SmtpClient smtpClient = new();
            try
            {
                await smtpClient.ConnectAsync(emailHost, emailPort, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailAddress, emailPassword);
                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}