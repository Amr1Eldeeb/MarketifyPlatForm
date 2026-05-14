using Marketify.Contracts.Product;
using Marketify.Entites;
using Marketify.Roles;
using Marketify.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;

namespace Marketify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService,ILogger<ProductController>logger)
        {
            _productService = productService;
            _logger = logger;
        }
        [HttpGet("test-claims")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult TestClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            var isSuperAdmin = User.IsInRole("SuperAdmin");

            return Ok(new
            {
                IsSuperAdmin = isSuperAdmin,
                Claims = claims
            });
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles =AppRoles.SuperAdmin)]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> Create([FromForm] CreateProduct request)
        {
            if (request.Images == null || !request.Images.Any())
            {
                return BadRequest("plz upload image");
            }
            var merchantId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            try
            {
                var result = await _productService.CreateProductAsync(request,merchantId!);

                if (result)
                {
                    return Ok(new { message = "Creation success" });
                }

                return BadRequest("Falid uploaded");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"something wrong  {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")] 

        public async Task<IActionResult> UpdateProduct(int id, [FromForm] EditProduct dto)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");

            try
            {
                var product = await _productService.EditProductAsync(id, dto);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (id == 0) return BadRequest("id is 0");

            try
            {
                var isDeleted = await _productService.DeleteProductAsync(id);

                if (isDeleted)
                {
                    return Ok(); 
                }

                return NotFound("Product not found in the database. It might have been already deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete product. It might be linked to other records. Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        [HttpGet("{Id}")]
       public async Task<IActionResult>GetById([FromRoute]int Id)
        {
            var productDto =await  _productService.GetByID(Id);
            if (productDto == null) return NotFound();
            return Ok(productDto);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()

        {
            _logger.LogWarning("Low stock for product");
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("GetProductsByCatID/{Id:int}")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductBycateId([FromRoute] int Id)
        {
            var products = await _productService.GetProductByCategory(Id);
            return Ok(products);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var results = await _productService.SearchProductsAsync(query);
            return Ok(results);
        }
        [HttpGet("Test-Rate")]
        [EnableRateLimiting("concurrency")]
        public async Task<IActionResult>Testt()
        {
            Thread.Sleep(6000);
            return Ok();
        }
        [HttpGet("ProductWithCaching")]
        public async Task <IActionResult>GetAll()
        {
            var products  = await _productService.GetAllProductsCaching();
            return Ok(products);    
        }
    }

}
