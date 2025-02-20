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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newComment = new CommentModel {
                    CreatedDate = DateTime.UtcNow,
                    UserId = userId,
                    PostId = postId,
                    Content = content
                };
                _context.Comments.Add(newComment);

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post != null) {
                    post.CommentCount++;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newComment;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while processing the comment request", ex);
            }
        }

        /*
         * 1.What if the user has multiple comments on the same post?
         * Should they be able to delete only one specific comment or all of them?
         * 2.What if an admin wants to delete a comment?
         * Admins should be able to delete any comment without checking userId.
         * 3.What if the comment doesn’t exist?
         * The system should handle cases where the comment is missing.
         */

        public async Task<bool> DeleteComment(Guid userId, Guid commentId, Guid postId) {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var comment = await _context.Comments.FirstOrDefaultAsync(l => l.UserId == userId && l.Id == commentId);
                if(comment == null)
                {
                    return false;
                }
                _context.Comments.Remove(comment);

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post != null)
                {
                    post.CommentCount--;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while deleting the comment request", ex);
            }
        }

        public async Task<bool> DeleteComments(Guid userId, Guid postId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var comments = await _context.Comments.Where(l => l.UserId == userId && l.PostId == postId).ToListAsync();
                if (comments == null)
                {
                    return false;
                }
                _context.Comments.RemoveRange(comments);

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post != null)
                {
                     post.CommentCount = post.CommentCount -  comments.Count ;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while deleting the comments request", ex);
            }
        }
    }
}
