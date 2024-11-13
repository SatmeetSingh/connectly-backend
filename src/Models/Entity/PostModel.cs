using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dating_app_backend.src.Models.Entity
{
    public interface IPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        Guid Id { get; set; }
        string Content { get; set; }
        string ImageUrl { get; set; } 
        int LikesCount { get; set; }
        int CommentCount { get; set; }
        Guid UserId { get; set; }
        int Share { get; set; }
        string? Location { get; set; }
        DateTime CreatedDate { get; set; } 
        DateTime? UpdatedDate { get; set; }
        UserModel User { get; set; }
    }

    [Table("Posts")]
    public class PostModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; } = String.Empty;
        [Required] 
        public string ImageUrl { get; set; } = String.Empty;
        public int LikesCount { get; set; }
        public int CommentCount { get; set; }
        public Guid UserId { get; set; }
        public int Share { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public UserModel User { get; set; }
    }
}
