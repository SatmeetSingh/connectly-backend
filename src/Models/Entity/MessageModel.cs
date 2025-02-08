using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public Guid RecieverId { get; set; }
        public String MessageContent { get; set; } = string.Empty;

        [Precision(3)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public MessageStatus Status { get; set; } 
    }


    public enum MessageStatus
    {
        Sent, Delivered, Read
    }
}
