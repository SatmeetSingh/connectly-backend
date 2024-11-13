using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Data.Common;
using Azure;
using Azure.Core;

namespace dating_app_backend.src.Service
{

    public class UserService
    {
        public readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            var Users = await _context.Users.ToListAsync();
            return Users;
        }

        public async Task<UserModel> GetUserById(Guid id) {
            if (id == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            } else
            {
               return await _context.Users.FirstOrDefaultAsync(e => e.Id == id);
            }
           
        }

        public async Task<UserModel> SignUpUser(SignUpUserDto userDto)
        {
             var User = new UserModel
             {
                 Username = userDto.Username,
                 Email = userDto.Email,
                 Password = HashPassword(userDto.Password)
             };

             _context.Users.Add(User);
             await _context.SaveChangesAsync();
             return User;        
        }

        public async Task<UserModel> LoginUser(LoginDto loginDto)
        {
           var user =await GetUserByEmail(loginDto.Email);
            if(user.Password.Equals(HashPassword(loginDto.Password)) ) {
                return user;
            } else
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
        }

        public async Task<UserModel> GetUserByEmail(string Email) {
            return await _context.Users.FirstOrDefaultAsync(e => e.Email.Equals(Email));
        }

        private string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }
        
    }
}
