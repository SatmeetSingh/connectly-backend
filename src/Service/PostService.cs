using dating_app_backend.src.DB;

namespace dating_app_backend.src.Service
{
    public class PostService
    {
        public readonly AppDbContext _context;

        public PostService(AppDbContext context) {
                 _context = context;
        }
    }
}
