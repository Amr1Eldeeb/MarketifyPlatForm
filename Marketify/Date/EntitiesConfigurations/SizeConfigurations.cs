using Marketify.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Drawing;
using Size = Marketify.Entites.Size;

namespace Marketify.Date.EntitiesConfigurations
{
    public class SizeConfigurations : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.HasData(
              new Size { Id = 1, Name = "Small" },
              new Size { Id = 2, Name = "Medium" },
              new Size { Id = 3, Name = "Large" }
           );
        }
    }
}
