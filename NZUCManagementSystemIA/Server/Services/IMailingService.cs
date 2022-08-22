using NZUCManagementSystemIA.Shared.MailingSystem;

namespace NZUCManagementSystemIA.Server.Services
{
    public interface IMailingService
    {
        void SendEmail(EmailDto emailDto);
    }
}