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

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts(int page, int limit)
        {
            var res = await _postService.GetAllPosts(page,limit);
            if (!res.Posts.Any())
            {
                return NotFound("No posts found.");
            }
            return Ok(new {status = "success", posts = res.Posts, page = page , limit = limit , totalPosts = res.TotalPosts }); 
        }

        //[HttpGet("{id}/likes")]
        //public async Task<IActionResult> PostLikes()
        //{
        //    try {
        //        return Ok();

        //    }catch(Exception ex)
        //    {
        //        return StatusCode(500, "An unexpected error occurred: " + ex.Message);
        //    }
        //}

        [EnableCors("AllowAll")]
        [HttpGet("{id}")]
        public async  Task<IActionResult> GetPostById(Guid id)
        {
            try
            { 
            var post = await _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
            }catch(Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [EnableCors("AllowAll")]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetPostsByUserId(Guid id)
        {
            try
            {
                var posts = await _postService.GetAllPostByUser(id);
                if (posts.Count == 0)
                {
                    return NotFound(new {message = "It looks like you haven't posted anything yet" });
                }
                return Ok(new { posts = posts , count = posts.Count});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDto createPost , Guid userId)
        {
            try
            {
                var post = await _postService.AddPost(createPost , userId);
                return Ok(new { message = "Post created Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePost(UpdatePostDto updatePost,Guid id) {
            try {
                var post = await _postService.UpdatePost(updatePost, id);
                return Ok(new { message = "Post updated Successfully",post = post });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id.GetType() == typeof(Guid))
            {
                try
                {
                   await _postService.DeletePost(id);
                   return Ok(new { message = "Post is deleted successfully" });
                }
                catch (Exception)
                {
                    return StatusCode(500, new { error = "An error occurred while processing your request. Please try again later." });    
                }
            }
            else
            {
                return Ok(new { message = "Id is not Guid" });
            }

        }
    }
}
