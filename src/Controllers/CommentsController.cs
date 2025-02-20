using dating_app_backend.src.Models.Entity;
using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace dating_app_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private CommentService _commentService { get; set; }

        public CommentsController(CommentService commentService) {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments() {
            try
            { 
            List<CommentModel> comments = await _commentService.GetAllComments();
             return Ok(new { Message = "Success", comments = comments, count = comments.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId)
        {
            try {
                List<CommentModel> commentsByPost = await _commentService.GetAllCommentByPost(postId);
                return Ok(new { Message = "Success", comments = commentsByPost, count = commentsByPost.Count });

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("{userId}/add/{postId}")]
        public async Task<IActionResult> PostCommnentOnPost(Guid userId,Guid postId , string content) {
            try {
                  CommentModel PostComment = await _commentService.PostComment(userId, postId, content);

                return Ok(new { Message = "User Commented" , comment = PostComment});         
            }
            catch (Exception err)
            {
                return StatusCode(500, err);
            }
        }

        [HttpDelete("{userId}/comment/{commentId}")]
        public async Task<IActionResult> DeleteCommentOnPost(Guid userId , Guid commentId, Guid postId)
        {
            try {
                var res = await _commentService.DeleteComment(userId, commentId, postId);
                if (!res)
                {
                    return NotFound("Comment not found.");
                }
                return Ok(new { message = "Comment Deleted Successfully" });
            }catch(Exception err)
            {
                return StatusCode(500, err);
            }
        }

        [HttpDelete("{userId}/post/{postId}")]
        public async Task<IActionResult> DeleteAllCommentsOnPostForUser(Guid userId, Guid postId)
        {
            try
            {
                var res = await _commentService.DeleteComments(userId, postId);
                if (!res)
                {
                    return NotFound("Comment not found.");
                }
                return Ok(new { message = "All User's Comments for certain post has been deleted Successfully" });
            }
            catch (Exception err)
            {
                return StatusCode(500, err);
            }
        }
    }

}
