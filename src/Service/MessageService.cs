using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.Service
{
    public class MessageService
    {
        private AppDbContext _context;

        private readonly IHubContext<MessageHub> _messageHubContext;

        public MessageService(AppDbContext context , IHubContext<MessageHub> messageHubContext)
        {
            _context = context;
            _messageHubContext = messageHubContext; 
        }

        /*
         * How Will This Work?
         * User connects to SignalR Hub → ConnectionId is stored in UserModel.
         * User sends a message → If the receiver is online (ConnectionId exists), message is sent instantly.
         * If the receiver is offline, message is saved in the database without real-time delivery.
         * When the receiver comes online, they will retrieve unread messages from the database.
         */
        public async Task<MessageResponseDto> SendMessageAsync(SendMessageDto messageDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sender = await _context.Users.FindAsync(messageDto.SenderId);
                var receiver = await _context.Users.FindAsync(messageDto.ReceiverId);

                if (sender == null || receiver == null)
                    throw new Exception("Sender or receiver does not exist.");

                if (string.IsNullOrWhiteSpace(messageDto.MessageContent))
                    throw new ArgumentException("Message content cannot be empty.");

                var message = new MessageModel
                {
                    SenderId = messageDto.SenderId,
                    ReceiverId = messageDto.ReceiverId,
                    MessageContent = messageDto.MessageContent,
                    SentAt = DateTime.UtcNow,
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Commit Transaction
                await transaction.CommitAsync();

                var messageResponseDto = new MessageResponseDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    MessageContent = message.MessageContent,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead
                };

                // 🔴 Real-Time Messaging via SignalR
                if (!string.IsNullOrEmpty(receiver.ConnectionId))
                {
                    await _messageHubContext.Clients.User(message.ReceiverId.ToString())
                    .SendAsync("ReceiveMessage", messageResponseDto);
                }
                return messageResponseDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback on failure
                throw new Exception("Database error occurred while sending the message.", ex);
            }
        }
    }
}
