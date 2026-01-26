using ECom.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(n => n.Name).IsRequired();
            builder.Property(d => d.Description).IsRequired();
            builder.Property(p => p.NewPrice).HasColumnType("decimal(18,2)");
            builder.HasData(new Product { Id = 1, Name = "Test Product", Description = "Description Test of Product 1",CategoryId=1, NewPrice = 12 });

        }
    }
}
