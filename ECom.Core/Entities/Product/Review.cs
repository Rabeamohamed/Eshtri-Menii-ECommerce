namespace ECom.Core.Entities.Product
{
    public class Review : BaseEntity<int>
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public string UserId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual AppUser User { get; set; }
    }
}
