using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dating_app_backend.src.Models.Entity
{
    [Table("Message")]
    public class MessageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid SenderId { get; set; }
        [JsonIgnore]
        public UserModel Sender { get; set; } = null!;

        [Required]
        public Guid ReceiverId { get; set; }

        [JsonIgnore]
        public UserModel Receiver { get; set; } = null!;


        public String MessageContent { get; set; } = string.Empty;

        [Precision(3)]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;  // Track if the message has been read
    }

}
