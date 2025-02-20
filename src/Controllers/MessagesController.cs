using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Service;

namespace dating_app_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private MessageService _messageService { get; set; }

        public MessagesController(MessageService messageService) {
            _messageService = messageService;
        }

        [HttpPost("sent")]
        public async Task<IActionResult> SendMessageToUserById([FromBody] SendMessageDto messageDto)
        {
            try {
                if (messageDto == null)
                    return BadRequest("Invalid message data");

                var messageResponse = await _messageService.SendMessageAsync(messageDto);
                return Ok(messageResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) {
                throw new Exception($"Internal Error Exception {ex}");
            }
        }


       
    }
}
