using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.Service
{
    public class PostService
    {
        public readonly AppDbContext _context;

        public PostService(AppDbContext context) {
                 _context = context;
        }

        public async Task<List<PostModel>> GetAllPosts()
        {
            var Posts  =await _context.Posts.ToListAsync();
            return Posts;
        }

        public async Task AddPost()
        {

        }
    }
}
