using NZUCManagementSystemIA.Shared.MailingSystem;

namespace NZUCManagementSystemIA.Client.Interfaces
{
    public interface IMailingServiceClient
    {
        void SendMailAsync(string url, EmailDto email);
    }
}