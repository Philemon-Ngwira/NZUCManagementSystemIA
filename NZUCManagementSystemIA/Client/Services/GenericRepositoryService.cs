using NZUCManagementSystemIA.Client.Interfaces;
using System.Net.Http.Json;

namespace NZUCManagementSystemIA.Client.Services
{
    public class GenericRepositoryService : IGenericRepositoryService
    {
        private readonly HttpClient _httpClient;

        public GenericRepositoryService(HttpClient Client)
        {
            _httpClient = Client;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string url) where T : class
        {
            try
            {
                var res = await _httpClient.GetFromJsonAsync<List<T>>(url);
                return res;
            }
            catch (Exception ex)

            {

                var _ = ex.Message;
                throw;
            }
        }





        //public async Task<Transaction> SaveAsync(string url, Transaction transaction)
        //{


        //    var result = await _httpClient.PostAsJsonAsync(url, transaction);
        //    return await result.Content.ReadFromJsonAsync<Transaction>();


        //}

        //public async Task<MasterTable> SaveMasterTableAsync(string url, MasterTable masterTable)
        //{


        //    var result = await _httpClient.PostAsJsonAsync(url, masterTable);
        //    return await result.Content.ReadFromJsonAsync<MasterTable>();


        //}
        public async Task<T?> SaveAllAsync<T>(string url, T value)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync(url, value);
                return await result.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }





        }


        public async Task<IEnumerable<T>> GetByIdAsync<T>(string url) where T : class, new()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<T>>(url);

                return result;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }
        }

        public async Task<T?> UpdateAsync<T>(string url, T value) where T : class
        {
            try
            {
                var updatedata = await _httpClient.PutAsJsonAsync(url, value);

                return await updatedata.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string api)
        {
            try
            {
                var deletedata = await _httpClient.DeleteAsync(api);
                //deletedata.EnsureSuccessStatusCode();
                var datadeleted = await deletedata.Content.ReadFromJsonAsync<bool>();

                return datadeleted;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }
        }
        //public async Task<List<MasterTable>> GetMasterTable(string url)
        //{
        //    try
        //    {
        //        return await _httpClient.GetFromJsonAsync<List<MasterTable>>(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = ex.Message;
        //        throw;
        //    }
        //}
        //public async Task<Site> SaveSiteAsync(string url, Site site)
        //{


        //    var result = await _httpClient.PostAsJsonAsync(url, site);
        //    return await result.Content.ReadFromJsonAsync<Site>();


        //}


        public async Task<T?> GetAllReturnModelAsync<T>(string url) where T : class
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(url);
            }
            catch (Exception ex)

            {
                var _ = ex.Message;
                throw;
            }
        }

    }
}
