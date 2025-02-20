using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace dating_app_backend.src.Models.Dto
{
    public class SendMessageDto
    {
        [Required]
        public Guid SenderId { get; set; }
       
        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
        public string MessageContent { get; set; } = string.Empty;

    }

    public class MessageResponseDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
