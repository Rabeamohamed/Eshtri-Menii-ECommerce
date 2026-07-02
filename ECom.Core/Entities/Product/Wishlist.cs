namespace ECom.Core.Entities.Product
{
    public class Wishlist : BaseEntity<int>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }

        // Navigation properties
        public virtual AppUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}
