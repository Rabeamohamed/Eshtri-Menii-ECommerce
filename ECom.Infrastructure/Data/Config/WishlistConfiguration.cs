
using ECom.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECom.Infrastructure.Data.Config
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasIndex(w => new { w.UserId, w.ProductId }) // Create a composite index on UserId and ProductId
                .IsUnique();

            //builder.Property(a => a.AddedAt).IsRequired();

            builder.HasOne(w => w.User)
                .WithMany() // Assuming AppUser does not have a collection of Wishlists
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete when the user is deleted

            builder.HasOne(w => w.Product)
                .WithMany() // Assuming Product does not have a collection of Wishlists
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete when the product is deleted
        }
    }
}
