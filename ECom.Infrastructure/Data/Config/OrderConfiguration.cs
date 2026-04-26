using ECom.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECom.Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            // Relations between Orders and Shipping Address
            builder.OwnsOne(x=>x.ShippingAddress, // One to one Relation
                n => { n.WithOwner(); });

            //Many to Many With OrderItems
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Convert the Enum to String
            builder.Property(s => s.Status).HasConversion(x => x.ToString(),
                x => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), x));

            builder.Property(t => t.SubTotal).HasColumnType("decimal(18,2)");
        }
    }
}
