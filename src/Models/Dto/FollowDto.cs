using dating_app_backend.src.Models.Entity;
using System.Text.Json.Serialization;

namespace dating_app_backend.src.Models.Dto
{
    public class FollowDto
    {
        public Guid FollowerId { get; set; } 
        public Guid FolloweeId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
