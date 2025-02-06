﻿using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
/*
 * POST / likes(to add a like),
 * DELETE / likes /{ id} (to remove a like),
 * GET /likes?postId={postId} (to fetch likes for a specific post).
 */
namespace dating_app_backend.src.Service
{
    public class LikesService
    {
        private readonly AppDbContext _context;

        public LikesService(AppDbContext context)
        {
            _context = context;

        }
        public async Task<List<LikesModel>> GetLikes()
        {
            return await _context.Likes.ToListAsync();
        }

        public async Task<List<LikesModel>> GetLikesByPost(Guid postId)
        {
            if (postId == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }
            else
            { 
               var LikesByUserId = await _context.Likes.Where(l => l.PostId == postId).ToListAsync();
                if (LikesByUserId.Count == 0)
                {
                    return new List<LikesModel>();
                }
                return LikesByUserId;
            }
        }

        public async Task<List<UserLikesDto>> GetUsersWhoLikedPost(Guid postId)
        {
            if (postId == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }
            else
            {
                var UsersWhoLikedPost = await _context.Likes
                                                      .Where(l => l.PostId == postId)
                                                      .Join(_context.Users,
                                                             l => l.UserId,
                                                             u => u.Id,
                                                             (l, u) => new UserLikesDto {
                                                                 Id = u.Id,
                                                                 Name = u.Name,
                                                                 Username = u.Username,
                                                                 ProfilePicture = u.ProfilePicture,
                                                                 CreatedDateValue = l.CreatedDate
                                                             })
                                                      .ToListAsync();
                if (UsersWhoLikedPost.Count == 0)
                {
                    return new List<UserLikesDto>();
                }
                return UsersWhoLikedPost;
            }
        }

        public async Task<List<GetPostDto>> GetPostsLikedByUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }
            else
            {
                var PostsLikedByUser = await _context.Likes
                                                      .Where(l => l.UserId == userId)
                                                      .Join(_context.Posts,
                                                             l => l.PostId,
                                                             u => u.Id,
                                                             (l, u) => new GetPostDto
                                                             {
                                                                 Id = u.Id,
                                                                 Content = u.Content,
                                                                 ImageUrl = u.ImageUrl,
                                                                 LikesCount = u.LikesCount,
                                                                 CommentCount = u.CommentCount,
                                                                 ShareCount =   u.ShareCount,
                                                                 Location = u.Location,
                                                                 UserId = u.UserId,
                                                                 Share = u.Share,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate

                                                             })
                                                      .ToListAsync();
                if (PostsLikedByUser.Count == 0)
                {
                    return new List<GetPostDto>();
                }
                return PostsLikedByUser;
            }
        }

        public async Task<LikesModel> GetLikesByUser(Guid postId , Guid userId) {
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (existingLike == null)
            {
                throw new BadHttpRequestException($"userId: {userId} have  not liked  the post with postId: {postId}");
            }

            return existingLike;
        }

        public async Task<bool> AddPostLikeAsync(Guid postId,Guid userId)
        {
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                throw new BadHttpRequestException("You have already liked this post.");
            }

            var like = new LikesModel
            {
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
                PostId = postId,
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikePostAsync(Guid userId, Guid postId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            if (like == null)
            {
                return false;
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HasUserLikedPostAsync(Guid userId, Guid postId)
        {
            return await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.PostId == postId);
        }
    }
}
