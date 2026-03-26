using Marketify.Contracts.Category;
using Marketify.Date;
using Marketify.Entites;
using Microsoft.EntityFrameworkCore;

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
            var dto = new GetCategoryByIdDTO(category.Name);
            
            return dto;
        }

        public async Task<IEnumerable<GetCategoryByIdDTO>> GetAllCategories()
        {
            var categoris = await _context.Categories
        .Select(x => new GetCategoryByIdDTO(x.Name))
        .ToListAsync();

            return categoris;



        }
    }
}
