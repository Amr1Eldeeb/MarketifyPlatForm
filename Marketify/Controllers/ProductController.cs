using Marketify.Contracts.Product;
using Marketify.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")] //detemine action allow files
        public async Task<IActionResult> Create([FromForm] CreateProduct request)
        {
            if (request.Images == null || !request.Images.Any())
            {
                return BadRequest("plz upload image");
            }

            try
            {
                var result = await _productService.CreateProductAsync(request);

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
        public async Task<IActionResult> DeleteProduct([FromRoute]int id)
        {
            if (id == 0) return BadRequest("id is 0");
          
            var IsDeleted =await  _productService.DeleteProductAsync(id);
            if(IsDeleted )
            {
            return Ok();
            }
                return BadRequest("SomeThins Wrong Error");
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
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

    }

}
