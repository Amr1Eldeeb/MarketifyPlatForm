using Marketify.Contracts.Category;
using Marketify.Date;
using Marketify.Entites;

namespace Marketify.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
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

    }
}
