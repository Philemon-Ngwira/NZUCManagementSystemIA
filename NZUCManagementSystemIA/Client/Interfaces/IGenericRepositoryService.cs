namespace NZUCManagementSystemIA.Client.Interfaces
{
    public interface IGenericRepositoryService
    {
        Task<bool> DeleteAsync(string api);
        Task<IEnumerable<T>> GetAllAsync<T>(string url) where T : class;
        Task<T?> GetAllReturnModelAsync<T>(string url) where T : class;
        Task<IEnumerable<T>> GetByIdAsync<T>(string url) where T : class, new();
        Task<T?> SaveAllAsync<T>(string url, T value);
        Task<T?> UpdateAsync<T>(string url, T value) where T : class;
    }
}