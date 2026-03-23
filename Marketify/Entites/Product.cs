namespace Marketify.Entites
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        
        public int CategoryId { get; set; }
        public Category? Category { get; set; } 

        //public string? VendorId { get; set; }
        //public ApplicationUser? Vendor { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}
