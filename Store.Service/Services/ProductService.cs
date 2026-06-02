using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // ==================================================
        private ProductSummaryDto MapToSummaryDto(ProductCatalog product)
        {
            return new ProductSummaryDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryName = product.ProductCategory?.CategoryName ?? string.Empty,
                ThumbnailUrl = product.ProductImages
                                         .OrderBy(i => i.ImageOrder)
                                         .FirstOrDefault()?.ImageURL
            };
        }

        private ProductDetailDto MapToDetailDto(ProductCatalog product)
        {
            return new ProductDetailDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryID = product.CategoryID,
                CategoryName = product.ProductCategory?.CategoryName ?? string.Empty,
                Images = product.ProductImages
                                         .OrderBy(i => i.ImageOrder)
                                         .Select(i => new ProductImageDto
                                         {
                                             ImageID = i.ImageID,
                                             ImageURL = i.ImageURL,
                                             ImageOrder = i.ImageOrder
                                         }).ToList()
            };
        }

        private ProductCatalog MapToEntity(ProductCreateDto dto)
        {
            return new ProductCatalog
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                QuantityInStock = dto.QuantityInStock,
                CategoryID = dto.CategoryID,
                ProductImages = dto.Images.Select(i => new ProductImages
                {
                    ImageURL = i.ImageURL,
                    ImageOrder = i.ImageOrder
                }).ToList()
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<ProductSummaryDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithDetailsAsync();
            return products.Select(p => MapToSummaryDto(p));
        }

        public async Task<ProductSummaryDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product == null) return null;
            return MapToSummaryDto(product);
        }

        public async Task<ProductSummaryDto> CreateAsync(ProductCreateDto createDto)
        {
            var product = MapToEntity(createDto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return MapToSummaryDto(product);
        }

        public async Task<ProductSummaryDto?> UpdateAsync(int id, ProductUpdateDto updateDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return null;

            product.ProductName = updateDto.ProductName;
            product.Description = updateDto.Description;
            product.Price = updateDto.Price;
            product.QuantityInStock = updateDto.QuantityInStock;
            product.CategoryID = updateDto.CategoryID;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return MapToSummaryDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // IProductService Specific Methods
        // ==================================================
        public async Task<ProductDetailDto?> GetDetailByIdAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);
            if (product == null) return null;
            return MapToDetailDto(product);
        }

        public async Task<IEnumerable<ProductSummaryDto>> GetByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
            return products.Select(p => MapToSummaryDto(p));
        }

        public async Task<IEnumerable<ProductSummaryDto>> SearchByNameAsync(string name)
        {
            var products = await _unitOfWork.Products.SearchByNameAsync(name);
            return products.Select(p => MapToSummaryDto(p));
        }

        public async Task<IEnumerable<ProductSummaryDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var products = await _unitOfWork.Products.GetByPriceRangeAsync(minPrice, maxPrice);
            return products.Select(p => MapToSummaryDto(p));
        }

        public async Task<ProductImageDto> AddImageAsync(int productId, ProductImageCreateDto imageDto)
        {
            var image = new ProductImages
            {
                ProductID = productId,
                ImageURL = imageDto.ImageURL,
                ImageOrder = imageDto.ImageOrder
            };

            await _unitOfWork.Products.AddImageAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return new ProductImageDto
            {
                ImageID = image.ImageID,
                ImageURL = image.ImageURL,
                ImageOrder = image.ImageOrder
            };
        }

        public async Task<bool> RemoveImageAsync(int imageId)
        {
            var result = await _unitOfWork.Products.RemoveImageAsync(imageId);
            if (!result) return false;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            await _unitOfWork.Products.UpdateStockAsync(productId, quantity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
