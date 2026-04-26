using System.ComponentModel.DataAnnotations;

namespace ECom.Application.DTO.Review
{
    public record CreateReviewDto
    {
        [Required]
        public int ProductId { get; init; }

        [Required]
        [Range(1, 5,ErrorMessage ="Rating Must be between 1 and 5")]
        public int Rating { get; init; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; init; }
    }
}
