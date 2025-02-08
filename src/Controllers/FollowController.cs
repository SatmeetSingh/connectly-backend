using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dating_app_backend.src.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private FollowService _followService {  get; set; }

        public FollowController(FollowService followService)
        {
            _followService = followService;
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetUsersFollowers(Guid userId) {
            try
            {
                var follower = await _followService.UserFollowers(userId);
                return Ok(new { message = "Users Followers are being displayed ", data = follower });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected Error occur : {ex}");
            }
        }

        [HttpGet("{userId}/following")]
        public async Task<IActionResult> GetUsersFollowing(Guid userId)
        {
            try
            {
                var following = await _followService.UserFollowing(userId);
                return Ok(new { message = "Users Following is being displayed", data = following });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected Error occur : {ex}");
            }
        }

        [HttpPost("{followerId}/follow/{followeeId}")]
        public async Task<IActionResult> FollowOtherUsers(Guid followerId,Guid followeeId) {
            try {
               var res =   await _followService.FollowOtherUser(followerId, followeeId);
                return Ok(new { message = "Success"  , data = res});
            }catch(Exception ex) {
                return StatusCode(500, $"Unexpected Error occur : {ex}");
            }
        }

        [HttpPost("{followerId}/unfollow/{followeeId}")]
        public async Task<IActionResult> UnFollowOtherUsers(Guid followerId,Guid followeeId)
        {
            try {
                var res = await _followService.UnFollowOtherUser(followerId, followeeId);
                return Ok(new { message = res });
            }catch(Exception ex) {
                return StatusCode(500, $"Unexpected Error occur : {ex}");
            }
        }
    }
}
