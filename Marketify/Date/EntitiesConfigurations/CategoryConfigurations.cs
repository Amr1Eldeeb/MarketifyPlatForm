using Marketify.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketify.Date.EntitiesConfigurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category { Id = 1, Name = "Electronics" },
    new Category { Id = 2, Name = "Fashion" },
    new Category { Id = 3, Name = "Home & Kitchen" },
    new Category { Id = 4, Name = "Beauty & Personal Care" },
    new Category { Id = 5, Name = "Sports & Fitness" },
    new Category { Id = 6, Name = "Books" },
    new Category { Id = 7, Name = "Toys & Games" },
    new Category { Id = 8, Name = "Automotive" },
    new Category { Id = 9, Name = "Health" },
    new Category { Id = 10, Name = "Grocery" },
    new Category { Id = 11, Name = "Mobile Phones" },
    new Category { Id = 12, Name = "Laptops" },
    new Category { Id = 13, Name = "Cameras" },
    new Category { Id = 14, Name = "Headphones" },
    new Category { Id = 15, Name = "Men Clothing" },
    new Category { Id = 16, Name = "Women Clothing" },
    new Category { Id = 17, Name = "Kids Clothing" },
    new Category { Id = 18, Name = "Shoes" },
    new Category { Id = 19, Name = "Bags & Accessories" },
    new Category { Id = 20, Name = "Furniture" },
    new Category { Id = 21, Name = "Home Decor" },
    new Category { Id = 22, Name = "Kitchen Appliances" },
    new Category { Id = 23, Name = "Bedding" },
    new Category { Id = 24, Name = "Skincare" },
    new Category { Id = 25, Name = "Makeup" },
    new Category { Id = 26, Name = "Hair Care" },
    new Category { Id = 27, Name = "Fragrances" },
    new Category { Id = 28, Name = "Gaming Consoles" },
    new Category { Id = 29, Name = "Office Supplies" },
    new Category { Id = 30, Name = "Pet Supplies" }

                );
        }
    }
}
