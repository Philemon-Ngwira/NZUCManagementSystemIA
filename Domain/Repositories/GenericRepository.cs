using Microsoft.EntityFrameworkCore;
using Domain.Data;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public class GenericRepository :IGenericRepository
    {
        private readonly NZUCMANAGEMENTSYSTEMContext _context;
        public GenericRepository(NZUCMANAGEMENTSYSTEMContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<T> GetByIdAsync<T>(int Id) where T : class, new()
        {
            try
            {
                var result = await _context.Set<T>().FindAsync(Id);
                if (result == null)
                    return new T();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> SaveAsync<T>(T value) where T : class
        {
            try
            {
                var result = await _context.AddAsync(value);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> UpdateAsync<T>(T value) where T : class
        {
            try
            {
                var result = _context.Update(value);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync<T>(Func<T, bool> predicate) where T : class
        {
            try
            {
                var resultToDelete = _context.Set<T>().Where(predicate).FirstOrDefault();
                if (resultToDelete == null)
                    return false;
                _context.Remove(resultToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
