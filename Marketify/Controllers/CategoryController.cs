using Marketify.Contracts.Category;
using Marketify.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
         public async Task<IActionResult>AddCategory([FromBody]CreateCategoryDto categoryDto)
        {
            var result =await _categoryService.CreateCategory(categoryDto);
            if(result) return Ok(result);
            return BadRequest();
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> EditCategory([FromRoute]int Id ,EditCategory editCategory)
        {
            var result = await _categoryService.EditCategory(Id, editCategory);
            if (result) return Ok(result);
            return BadRequest();
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult>SoftDeleteCategory([FromRoute]int Id)
        {
            var result = await _categoryService.SoftDelete(Id);
            if(result) return Ok(result);
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int Id)
        {
            var response = await _categoryService.GetCategoryById(Id);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult>GetAllCategoryes()
        {
            var result =await _categoryService.GetAllCategories();
            return Ok(result);
        }



    }
}
