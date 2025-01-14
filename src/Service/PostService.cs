﻿using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

        public async Task<List<PostModel>> GetAllPosts()
        {
            var Posts  =await _context.Posts.ToListAsync();
            return Posts;
        }

        public async Task<List<PostModel>> GetAllPostByUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }
            else
            {
                var Posts = await _context.Posts.Where(e => e.UserId == id).ToListAsync();
                if (Posts.Count == 0)
                {
                    return new List<PostModel>();
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

        public async Task<PostModel> AddPost(CreatePostDto createPost, Guid id)
        {
            var post = new PostModel
            {
                Content = createPost.Content,
                Location = createPost.Location ?? "",
                UserId = id
            };
            var filePath = await _fileService.SavePostAsync(createPost.Image);
            post.ImageUrl = filePath;

             _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task DeletePost(Guid id) {
            await _context.Posts.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
