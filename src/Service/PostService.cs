﻿using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace dating_app_backend.src.Service
{
    public class PostService
    {
        public readonly AppDbContext _context;
        public readonly FileService _fileService;

        public PostService(AppDbContext context,FileService fileService) {
            _context = context;
            _fileService = fileService;
        }

        public async Task<(int TotalPosts ,List<PostModel> Posts)> GetAllPosts(int page = 1, int limit = 10)
        {
            int Page = Math.Max(page, 1);

            var query =  _context.Posts.AsNoTracking();

            int totalPosts =await query.CountAsync();
            List<PostModel> posts =await query.OrderByDescending(p => p.CreatedDate)
                                            .Skip((Page - 1) * limit)
                                            .Take(limit)
                                            .ToListAsync();
            if (posts == null)
            {
                throw new KeyNotFoundException("No posts found.");
            }
            
            return ( totalPosts, posts);
        } 


        public async Task<List<PostModel>> GetAllPostByUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)");
            }
            else
            {
                var Posts = await _context.Posts.Where(e => e.UserId == id).ToListAsync();
                if (Posts.Count == 0)
                {
                    throw new KeyNotFoundException("It looks like you haven't posted anything yet");
                }
                return Posts;
            }
        }
        public async Task<PostModel> GetPostById(Guid id) {
            if (id == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }
            else
            { 
                var post =  await _context.Posts.FirstOrDefaultAsync(e => e.Id == id);
                if (post == null)
                {
                    throw new ArgumentNullException(nameof(post), "User cannot be null");
                }
                return post;
            }
        }


        public async Task<PostModel> UpdatePost(UpdatePostDto updatePost,Guid id)
        {
            var post = await GetPostById(id);
            if (post == null)
            {
                throw new KeyNotFoundException("post does not match");
            }
            if (updatePost.LikesCount.HasValue)
            {
                post.LikesCount = updatePost.LikesCount;
            }
            if (updatePost.CommentCount.HasValue)
            {
                post.CommentCount = updatePost.CommentCount;
            }
            if (updatePost.ShareCount.HasValue)
            {
                post.ShareCount = updatePost.ShareCount;
            }

            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<PostModel> AddPost(CreatePostDto createPost, Guid userId)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {

                var post = new PostModel
                {
                    Content = createPost.Content,
                    Location = createPost.Location ?? "",
                    UserId = userId
                };
                var filePath = await _fileService.SavePostAsync(createPost.Image);
                post.ImageUrl = filePath;

                var user =await _context.Users.FirstOrDefaultAsync(l => l.Id == userId);
                if(user != null)
                {
                   user.PostCount++;
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return post;
            }catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Transaction Failure:  {ex}");
            }
        }

        public async Task DeletePost(Guid id,Guid userId) {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.Posts.Where(p => p.Id == id && p.UserId == userId).ExecuteDeleteAsync();

                var user = await _context.Users.FirstOrDefaultAsync(l => l.Id == userId);
                if (user != null)
                {
                    user.PostCount--;
                }
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
