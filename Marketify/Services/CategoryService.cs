using Marketify.Contracts.Category;
using Marketify.Date;
using Marketify.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Marketify.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public CategoryService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<bool> CreateCategory(CreateCategoryDto Dto)
        {
            var Category = new Category
            {
                Name = Dto.Name
            };
            _context.Categories.Add(Category);
            var result = await _context.SaveChangesAsync();

            return result==1 ?true : false;
        }
        public async Task<bool>EditCategory(int Id, EditCategory Dto)
        {
            var category = _context.Categories.SingleOrDefault(x => x.Id == Id);
            if(Dto.Name !=category.Name)
            {
                category.Name = Dto.Name;
            };
              _context.Categories.Update(category);
            var result = _context.SaveChanges();
            return result == 1 ? true : false;

        }

        public async Task<bool> SoftDelete(int Id)
        {

            var category =_context.Categories.SingleOrDefault(x=>x.Id ==Id);

            if(category!=null)
            {
                category.IsDeleted = true;
            }
                var result = _context.SaveChanges();

            if(result ==0)return false;
            return true;


            
        }
        

        public async Task<GetCategoryByIdDTO> GetCategoryById(int Id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.Id == Id);
            var dto = new GetCategoryByIdDTO(category.Id ,category.Name);
            
            return dto;
        }

        public async Task<IEnumerable<GetCategoryByIdDTO>> GetAllCategories()
        {
            string cacheKey = "all_categories_list";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<GetCategoryByIdDTO>? categories))
            {
                categories = await _context.Categories
                    .Select(x => new GetCategoryByIdDTO(x.Id, x.Name!))
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetPriority(CacheItemPriority.High);

                _cache.Set(cacheKey, categories, cacheOptions);
            }

            return categories!;
        }
    }
}
