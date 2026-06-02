using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Get all records
        Task<IEnumerable<T>> GetAllAsync();

        // Get a single record by its ID
        Task<T?> GetByIdAsync(int id);

        // Add a new record
        Task AddAsync(T entity);

        // Update an existing record
        void Update(T entity);

        // Delete a record
        void Delete(T entity);
    }
}
