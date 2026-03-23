using Marketify.Contracts.Category;

namespace Marketify.Services
{
    public interface ICategoryService
    {
        Task<bool> CreateCategory(CreateCategoryDto Dto);
        Task<bool> EditCategory(int Id,EditCategory Dto);
    }
}
