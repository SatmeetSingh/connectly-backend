using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dating_app_backend.src.Models.Entity
{
     [Table("Follow")]
     public class FollowModel
     {
        public Guid FollowerId { get; set; }

        [JsonIgnore]
        public UserModel Follower { get; set; } = null!; 

        public Guid FolloweeId { get; set; }

        [JsonIgnore]
        public UserModel Following { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}


/**
  * FolloweeId would be A (the person being followed).
  * FollowerId would be B (the person doing the following).
  * public Guid UserId { get; set; } // FollowerId   
  * public Guid TargetUserId { get; set; } // Person being followed (followeeId)
 */