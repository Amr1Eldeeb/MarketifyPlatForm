namespace Marketify.Entites
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; } // هل هي الصورة الأساسية للمنتج؟

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
