namespace Domain.Interfaces
{
    public interface IGenericRepository
    {
        Task<bool> DeleteAsync<T>(Func<T, bool> predicate) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<T> GetByIdAsync<T>(int Id) where T : class, new();
        Task<T> SaveAsync<T>(T value) where T : class;
        Task<T> UpdateAsync<T>(T value) where T : class;
    }
}