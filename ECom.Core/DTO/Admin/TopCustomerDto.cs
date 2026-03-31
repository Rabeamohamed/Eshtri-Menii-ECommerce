namespace ECom.Core.DTO.Admin
{
    public record TopCustomerDto
    {
        public string UserId { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public int TotalOrders { get; init; }
        public decimal TotalSpent { get; init; }
    }
}
