using Marketify.Contracts.Product;
using Marketify.Contracts.Review;

namespace Marketify.Services
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(CreateProduct dto, string merchantId);
        Task<bool> EditProductAsync(int Id,EditProduct dto);
        Task<bool> DeleteProductAsync(int Id);
        Task<ProductResponseDto> GetByID(int Id);
        public Task<IEnumerable<ProductReadDto>> GetAllProductsAsync();
        public Task<IEnumerable<ProductReadDto>> GetProductByCategory(int?  categoryId);

        public  Task<IEnumerable<ProductReadDto>> SearchProductsAsync(string searchTerm);
        //Task<bool> AddReviewAsync(string userId, CreateReviewDto reviewDto);
        //Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);
    }
}
