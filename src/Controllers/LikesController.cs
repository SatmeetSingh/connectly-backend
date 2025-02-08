using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
       private LikesService _likesService { get; set; }

        public LikesController(LikesService likesService) {
            _likesService = likesService;
        }

        [HttpGet]
        public async Task<IActionResult> AllLikes() {
            try
            {
                var Likes = await _likesService.GetLikes();
                return Ok(new { Message = "Success" , likes = Likes , count = Likes.Count});
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{postId}/user/{userId}")]
        public async Task<IActionResult> GetPost(Guid userId, Guid postId)
        {
            try
            {

                if (userId == Guid.Empty || postId == Guid.Empty)
                {
                    return BadRequest("Invalid user or post ID.");
                }

                var success = await _likesService.GetLikesByUser(postId, userId);
              
                return Ok(new { message = "Success" , post = success });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error getting post for PostId: {postId}, UserId: {userId}" + ex);
            }
        }

        /* Allow users to see who liked a post */
        [HttpGet("{postId}/users")]
        public async Task<IActionResult> GetAllUserWhoLikedPost(Guid postId) {
            try {
                if ( postId == Guid.Empty)
                {
                    return BadRequest("Invalid  post ID.");
                }
                var res = await _likesService.GetUsersWhoLikedPost(postId);
                return Ok(new { message = "success" , users = res ,count = res.Count });
            }catch(Exception err)
            {
                return StatusCode(500, err);
            }
        }

        /* Allow users to see posts they liked: */
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllPostsLikedByPost(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid  post ID.");
                }
                var res = await _likesService.GetPostsLikedByUser(userId);
                return Ok(new { message = "success", posts = res, count = res.Count });
            }
            catch (Exception err)
            {
                return StatusCode(500, err);
            }
        }


        [HttpGet("{postId}")]
        public async Task<IActionResult>  LikesByPost(Guid postId)
        {
            try
            {
                var Likes = await _likesService.GetLikesByPost(postId);
                return Ok(new { Message = "Success", likes = Likes, count = Likes.Count });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("{postId}/like/{userId}")]
        public async Task<IActionResult> LikePost(Guid userId, Guid postId)
        {
            try
            {

                if (userId == Guid.Empty || postId == Guid.Empty)
                {
                    return BadRequest("Invalid user or post ID.");
                }

                var success = await _likesService.AddPostLikeAsync( postId, userId);
                if (!success)
                {
                    return Conflict("User has already liked this post.");
                }
                return Ok(new { Message = "User Liked"} );           
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, $"Error saving like for PostId: {postId}, UserId: {userId}"+ ex);
            }
            
        }


        [HttpDelete("{postId}/unlike/{userId}")]
        public async Task<IActionResult> UnlikePost(Guid userId, Guid postId)
        {
            if (userId == Guid.Empty || postId == Guid.Empty)
            {
                return BadRequest("Invalid user or post ID.");
            }

            var success = await _likesService.UnlikePostAsync(userId, postId);
            if (!success)
            {
                return NotFound("Like not found.");
            }

            return Ok(new { Message = "Post unliked successfully." });
        }
    }
}

