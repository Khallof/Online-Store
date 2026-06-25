using Store.Core.DTOs.Customer;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // Entity → DTO and DTO → Entity
        // ==================================================
        private CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                Username = customer.Username,
                Role= customer.Role,
               
            };
        }

        private Customer MapToEntity(CustomerCreateDto dto)
        {
            return new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Username = dto.Username,
                Password = dto.Password  // should be hashed before saving
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return customers.Select(c => MapToDto(c));
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return null;
            return MapToDto(customer);
        }

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto createDto)
        {
            var customer = MapToEntity(createDto);
            customer.Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password);


           
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(customer);
        }

        public async Task<CustomerDto?> UpdateAsync(int id, CustomerUpdateDto updateDto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return null;


            if (customer.Email != updateDto.Email)
            {
                var emailExists = await _unitOfWork.Customers
                                                   .EmailExistsAsync(updateDto.Email);
                if (emailExists)
                    throw new InvalidOperationException(
                        $"Email '{updateDto.Email}' is already taken.");
            }



            if (customer.Username != updateDto.Username)
            {
                var usernameExists = await _unitOfWork.Customers
                                                      .UsernameExistsAsync(updateDto.Username);
                if (usernameExists)
                    throw new InvalidOperationException(
                        $"Username '{updateDto.Username}' is already taken.");
            }

          
            customer.Name = updateDto.Name;
            customer.Email = updateDto.Email;
            customer.Phone = updateDto.Phone;
            customer.Address = updateDto.Address;
            customer.Username = updateDto.Username;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(customer);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // ICustomerService Specific Methods
        // ==================================================
        public async Task<CustomerDto?> GetByEmailAsync(string email)
        {
            var customer = await _unitOfWork.Customers.GetByEmailAsync(email);
            if (customer == null) return null;
            return MapToDto(customer);
        }

        public async Task<CustomerDto?> GetByUsernameAsync(string username)
        {
            var customer = await _unitOfWork.Customers.GetByUsernameAsync(username);
            if (customer == null) return null;
            return MapToDto(customer);
        }

        public async Task<CustomerDto?> GetWithOrdersAsync(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetWithOrdersAsync(customerId);
            if (customer == null) return null;
            return MapToDto(customer);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Customers.EmailExistsAsync(email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _unitOfWork.Customers.UsernameExistsAsync(username);
        }
    }
}
