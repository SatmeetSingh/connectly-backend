using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace dating_app_backend.src.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Bio { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [Precision(3)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Precision(3)]
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
        public int PostCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
