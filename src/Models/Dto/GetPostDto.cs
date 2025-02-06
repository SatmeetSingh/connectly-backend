using System.ComponentModel.DataAnnotations;

namespace dating_app_backend.src.Models.Dto
{
    public class GetPostDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Content { get; set; } = String.Empty;
        [Required]
        public string ImageUrl { get; set; } = String.Empty;
        public int? LikesCount { get; set; } = 0;
        public int? CommentCount { get; set; } = 0;
        public int? ShareCount { get; set; } = 0;
        public string Location { get; set; } = String.Empty;

        public Guid UserId { get; set; }
        public int Share { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } 
    }
}
