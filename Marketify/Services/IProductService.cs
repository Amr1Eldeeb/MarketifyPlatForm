using Marketify.Contracts.Product;

namespace Marketify.Services
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(CreateProduct dto);
        Task<bool> EditProductAsync(int Id,EditProduct dto);
        Task<bool> DeleteProductAsync(int Id);
        Task<ProductResponseDto> GetByID(int Id);
        public Task<IEnumerable<ProductReadDto>> GetAllProductsAsync();
        public Task<IEnumerable<ProductReadDto>> GetProductByCategory(int?  categoryId);
    }
}
