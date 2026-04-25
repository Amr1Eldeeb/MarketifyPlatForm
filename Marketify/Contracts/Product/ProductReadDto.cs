namespace Marketify.Contracts.Product
{
    public class ProductReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        public List<string> Sizes { get; set; } = new();
    }
}
