using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.MailingSystem;
using System.Net.Http.Json;

namespace NZUCManagementSystemIA.Client.Services
{
    public class MailingServiceClient : IMailingServiceClient
    {
        private readonly HttpClient _httpClient;
        public MailingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SendMailAsync(string url, EmailDto email)
        {
            try
            {
                var result = _httpClient.PostAsJsonAsync(url, email);
                //return await result.Content.ReadFromJsonAsync<EmailDto>();
            }
            catch (System.Exception ex)
            {
                var _ = ex.Message;
                throw;
            }




        }
    }
}
