using ECom.Core.Entities;
using ECom.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECom.Infrastructure.Data.Config
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            // Unique coupon code
            builder.HasIndex(c => c.Code).IsUnique();

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MinimumOrderAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MaxDiscountAmount)
                .HasColumnType("decimal(18,2)");

            // Store enum as string
            builder.Property(c => c.Type)
                .HasConversion(
                    x => x.ToString(),
                    x => (CouponType)Enum.Parse(typeof(CouponType), x));
        }
    }
}