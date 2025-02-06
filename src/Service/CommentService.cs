using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.Service
{
    public class CommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context )
        {
            _context = context;
            
        }

        public async Task<List<CommentModel>> GetAllComments(){
            return await _context.Comments.ToListAsync();   
        }

        public async Task<List<CommentModel>> GetAllCommentByPost(Guid postId)
        {
            if(postId == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }

            List<CommentModel> AllCommentsByPost =  await _context.Comments.Where(l => l.PostId == postId).ToListAsync();
            if (AllCommentsByPost.Count == 0)
            {
                return new List<CommentModel>();
            }
            return AllCommentsByPost;
        }

        public async Task<CommentModel> PostComment(Guid userId , Guid postId, string content ) {

            var newComment = new CommentModel {
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
                PostId = postId,
                Content = content
            };
            Console.WriteLine($"Created a comment: {newComment}");
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return newComment;
        }

        /*
         * 1.What if the user has multiple comments on the same post?
         * Should they be able to delete only one specific comment or all of them?
         * 2.What if an admin wants to delete a comment?
         * Admins should be able to delete any comment without checking userId.
         * 3.What if the comment doesn’t exist?
         * The system should handle cases where the comment is missing.
         */

        public async Task<bool> DeleteComment(Guid userId,Guid commentId) {
           var comment = await _context.Comments.FirstOrDefaultAsync(l => l.UserId == userId && l.Id == commentId);
            if(comment == null)
            {
                return false;
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteComments(Guid userId, Guid postId)
        {
            var comments = await _context.Comments.Where(l => l.UserId == userId && l.PostId == postId).ToListAsync();
            if (comments == null)
            {
                return false;
            }
            _context.Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
