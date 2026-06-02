using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    
    public interface IGenericService<TDto, TCreateDto, TUpdateDto>
    {
        // Get all records
        Task<IEnumerable<TDto>> GetAllAsync();

        // Get a single record by ID
        Task<TDto?> GetByIdAsync(int id);

        // Create a new record
        Task<TDto> CreateAsync(TCreateDto createDto);

        // Update an existing record
        Task<TDto?> UpdateAsync(int id, TUpdateDto updateDto);

        // Delete a record
        Task<bool> DeleteAsync(int id);
    }
}
