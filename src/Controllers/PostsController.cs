using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace dating_app_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        public PostService _postService { get; set; }
        public ILogger<PostsController> _logger { get; set; }

        public PostsController(PostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts(int page, int limit)
        {
            try
            {

            var res = await _postService.GetAllPosts(page,limit);
           
            return Ok(new {status = "success", posts = res.Posts, page = page , limit = limit , totalPosts = res.TotalPosts }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred: " + ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        [EnableCors("AllowAll")]
        [HttpGet("{id}")]
        public async  Task<IActionResult> GetPostById(Guid id)
        {
            try
            { 
            var post = await _postService.GetPostById(id);
          
            return Ok(post);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex){
                return BadRequest(new { message = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An unexpected error occurred: " + ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [EnableCors("AllowAll")]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetPostsByUserId(Guid id)
        {
            try
            {
                var posts = await _postService.GetAllPostByUser(id);
                return Ok(new { posts = posts , count = posts.Count});
            }
            catch(BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message}); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An unexpected error occurred: " + ex.Message,
                    stackTrace = ex.StackTrace
                });

            }
        }


        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPost , Guid userId)
        {
            try
            {
                var post = await _postService.AddPost(createPost , userId);
                return Ok(new { message = "Post created Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred: " + ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePost(UpdatePostDto updatePost,Guid id) {
            try {
                var post = await _postService.UpdatePost(updatePost, id);
                return Ok(new { message = "Post updated Successfully",post = post });
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred: " + ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id,Guid userId)
        {
            try
            {
                await _postService.DeletePost(id,userId);
                return Ok(new { message = "Post is deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred: " + ex.Message,
                    stackTrace = ex.StackTrace
                });    
            }
        }
    }
}
