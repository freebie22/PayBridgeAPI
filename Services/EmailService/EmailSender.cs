using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;

namespace PayBridgeAPI.Services.EmailService
{
    public class EmailSender : IEmailSender
    {

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(email, subject, message);
        }

        public async static Task Execute(string email, string subject, string message)
        {
            string fromEmail = "paybridgeservice@gmail.com";
            string fromPassword = "tsqgcvlcpdhgxewb";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(mailMessage);

        }
    }
}