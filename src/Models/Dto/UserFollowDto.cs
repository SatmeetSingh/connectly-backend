using System.ComponentModel.DataAnnotations;

namespace dating_app_backend.src.Models.Dto
{
    public class UserFollowDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public DateTime CreatedDateValue { get; set; }
    }
}
