using Mapster;
using Marketify.Contracts.Product;
using Marketify.Date;
using Marketify.Entites;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Marketify.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductService(ApplicationDbContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<bool> CreateProductAsync(CreateProduct dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                Images = new List<ProductImage>()
            };

            product.ProductSizes = dto.SelectedSizeIds
                .Select(id => new ProductSize { SizeId = id, Product = product })
                .ToList();

            for (int i = 0; i < dto.Images.Count; i++)
            {
                var formFile = dto.Images[i];
                if (formFile != null && formFile.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                    var folder = Path.Combine(_env.WebRootPath, "images");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var path = Path.Combine(folder, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await formFile.CopyToAsync(stream);

                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = "/images/" + fileName,
                        IsMain = (i == dto.MainImageIndex)
                    });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async  Task<bool> EditProductAsync(int Id,EditProduct dto)
        {
            var product = _context.Products.
                Include(p=>p.Images).Include(x=>x.ProductSizes).SingleOrDefault(x=>x.Id == Id);
            if (product == null)
                throw new Exception("Product not found");
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;

            if (dto.SelectedSizeIds!=null )
            {
                product.ProductSizes.Clear();
                foreach(var id in dto.SelectedSizeIds)
                {
                    product.ProductSizes.Add(new ProductSize { SizeId = id });
                }
            }
            if(dto.Images!=null && dto.Images.Any())
            {
                foreach (var img in product.Images)
                {
                    var oldPath = Path.Combine(_env.WebRootPath, "images", img.ImageUrl);
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                product.Images.Clear();

                for (int i = 0; i < dto.Images.Count; i++)
                {
                    var file = dto.Images[i];

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var path = Path.Combine(_env.WebRootPath, "images", fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);

                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = fileName,
                        IsMain = i == dto.MainImageIndex
                    });
                }
            }
            int result = await _context.SaveChangesAsync();
            if(result == 0) return false;

            return true;

        }

        public async  Task<bool> DeleteProductAsync(int Id)
        {
            var product = _context.Products.Include(x => x.Images).Include(p => p.ProductSizes)
                .SingleOrDefault(x=>x.Id ==Id);
            if(product is null)
            
                return false; 
            foreach(var image in product.Images)
            {
                var path = Path.Combine(_env.WebRootPath,"images",image.ImageUrl);
                if(File.Exists(path))
                    File.Delete(path);
            }    
            _context.Products.Remove(product);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<ProductResponseDto> GetByID(int Id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductSizes)
                .SingleOrDefaultAsync(p => p.Id == Id);

            if (product == null) return null;
            string baseUrl = "http://localhost:4000"; 
            var orderedImages = product.Images?
                .OrderByDescending(img => img.IsMain)
                .Select(img => $"{baseUrl}{img.ImageUrl}") 
                .ToList() ?? new List<string>();


            var sizesList = product.ProductSizes?
                .Select(s => s.SizeId.ToString()) 
                .ToList() ;

            var dto = new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId,
                orderedImages,
                sizesList
            );

            return dto;
        }
    }
}
