using dating_app_backend.src.Models.Entity;
using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.GetAllPosts();
            if(posts.Count == 0)
            {
                return Ok(new { status = "success", posts = posts, message = "No posts found" });
            }
            return Ok(new {status = "success", posts = posts, count = posts.Count }); 
        }
    }
}
