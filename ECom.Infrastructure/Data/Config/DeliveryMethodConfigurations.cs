using ECom.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECom.Infrastructure.Data.Config
{
    public class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.HasData(
    new DeliveryMethod
    {
        Id = 1,
        DeliveryTime = "one week",
        Description = "Fastest Delivery in world",
        Name = "Mogz",
        Price = 200
    },
    new DeliveryMethod
    {
        Id = 2,
        DeliveryTime = "2 week",
        Description = "Hello with Fastest Delivery in Nahya",
        Name = "DHL",
        Price = 300
    }
);
        }
    }
}
