using Marketify.Contracts.Authenthication;
using Marketify.Contracts.Category;
using Marketify.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Marketify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly JwtOptions _jwtOptions;

        public CategoryController(ICategoryService categoryService,
            IConfiguration configuration,
            IOptions<JwtOptions>jwtOptions)
        {
            _categoryService = categoryService;
            this._configuration = configuration;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost]
         public async Task<IActionResult>AddCategory([FromBody]CreateCategoryDto categoryDto)
        {
            var result =await _categoryService.CreateCategory(categoryDto);
            if(result) return Ok(result);
            return BadRequest();
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> EditCategory([FromRoute]int Id ,[FromBody]EditCategory editCategory)
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult>GetAllCategoryes()
        {
            var result =await _categoryService.GetAllCategories();
            return Ok(result);
        }
        [HttpGet("Test")]
        public IActionResult Test()
        {
          
            
            return Ok(_jwtOptions.ExpireyMinutes);
        }

}
}
