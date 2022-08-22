using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using NZUCManagementSystemIA.Shared.MailingSystem;

namespace NZUCManagementSystemIA.Server.Services
{
    public class MailingService : IMailingService
    {
        public void SendEmail(EmailDto emailDto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("assetrisknotifier@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emailDto.To));
            email.Subject = emailDto.EmailSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailDto.EmailBody };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("nzucmanagementsystem@gmail.com", "pskmwjqebkmoyvqn");
            smtp.Send(email);
            smtp.Disconnect(true);


            //_emailService.SendEmail(email);


        }
    }
}
