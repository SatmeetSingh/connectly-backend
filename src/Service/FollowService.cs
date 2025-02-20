using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.Service
{
    public class FollowService
    {
        private readonly AppDbContext _context;

        public FollowService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<UserFollowDto>> UserFollowers(Guid userId)
        {
            var followers = await _context.Follows.Where(f => f.FolloweeId == userId)
                                            .Join(_context.Users,
                                                   l => l.FollowerId,
                                                   u => u.Id,
                                                   (l,u) => new UserFollowDto
                                                   {
                                                       Id = u.Id,
                                                       Name = u.Name,
                                                       Username = u.Username,
                                                       ProfilePicture = u.ProfilePicture,
                                                       CreatedDateValue = l.CreatedDate
                                                   }).ToListAsync();

            return followers;
        }

        public async Task<List<UserFollowDto>> UserFollowing(Guid userId)
        {
            var following = await _context.Follows.Where(f => f.FollowerId == userId)
                                            .Join(_context.Users,
                                                   l => l.FolloweeId,
                                                   u => u.Id,
                                                   (l, u) => new UserFollowDto
                                                   {
                                                       Id = u.Id,
                                                       Name = u.Name,
                                                       Username = u.Username,
                                                       ProfilePicture = u.ProfilePicture,
                                                       CreatedDateValue = l.CreatedDate
                                                   }).ToListAsync();

            return following;
        }

        public async Task<Boolean> CheckFollowStatus(Guid followerId,Guid followeeId)
        {
            var status = await _context.Follows.AnyAsync(l => l.FollowerId == followerId && l.FolloweeId == followeeId);
            if (status)
            {
                return true;
            }else
            {
                return false;
            }
        }

        public async Task<FollowModel> FollowOtherUser(Guid followerId , Guid followeeId) {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                if (followerId == followeeId)
                {
                    throw new InvalidOperationException("A user cannot follow themselves.");
                }

                // Check if user already following the target
                var follow = await _context.Follows
                                           .AnyAsync(l => l.FollowerId == followerId && l.FolloweeId == followeeId);
                                          

                // if follow already exist 
                if (follow) {
                    throw new Exception("User is alredy following the Target");
                }

                // Create the follow object ot insert in db
                var newFollow = new FollowModel
                {
                   FollowerId = followerId,
                    FolloweeId = followeeId,
                   CreatedDate = DateTime.UtcNow
                };

                // Add to followModel table
                await _context.Follows.AddAsync(newFollow);

                //  update user Table
                var targetUser = await _context.Users.FirstOrDefaultAsync(t => t.Id == followeeId);
                if(targetUser != null)
                {
                    targetUser.FollowersCount++;
                }
                else
                {
                    throw new Exception("Target user not found.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == followerId);
                if(user != null)
                {
                    user.FollowingCount++;
                }
                else
                {
                    throw new Exception("User not found.");
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return newFollow;
            } catch(Exception err)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while processing the follow operation", err);
            }
        }

        public async Task<string> UnFollowOtherUser(Guid followerId,Guid followeeId) {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try {

                var follow = await _context.Follows.FirstOrDefaultAsync(l => l.FollowerId == followerId && l.FolloweeId == followeeId);
                if (follow == null)
                {
                    throw new Exception("User has not followed the target yet");
                }
                 _context.Follows.Remove(follow);

                var targetUser = await _context.Users.FirstOrDefaultAsync(t => t.Id == followeeId);
                if (targetUser != null)
                {
                    targetUser.FollowersCount--;
                } else
                {
                    throw new Exception("Target user not found.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == followerId);
                if (user != null)
                {
                    user.FollowingCount--;
                } else
                {
                    throw new Exception("User not found.");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "User has successfully Unfollowed the target";
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while processing the unfollow operation", ex);
            }
        }

    }
}
