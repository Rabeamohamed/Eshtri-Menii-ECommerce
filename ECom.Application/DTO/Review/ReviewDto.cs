namespace ECom.Application.DTO.Review
{
    public record ReviewDto
    {
        public int Id { get; init; }
        public string Comment { get; init; }
        public int Rating { get; init; }
        public bool IsVerifiedPurchase { get; init; }
        public string UserName { get; init; }      // display who wrote it
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
