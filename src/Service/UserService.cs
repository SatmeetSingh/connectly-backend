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

/*
 *  _context refers to an instance of your Entity Framework DbContext. 
 *  It is used to interact with the database through various DbSets that represent tables in the database. 
 *  Typically, _context is used to query, update, and save data to a database.
 */

namespace dating_app_backend.src.Service
{

    public class UserService
    {
        public readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));   
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
            var user = await GetUserByEmail(loginDto.Email); 
            if (user == null)
            {
                throw new KeyNotFoundException("user does not match");
            }
            if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password)) {
                return user;
            } else
            {
                throw new UnauthorizedAccessException("Incorrect Password, Please try again.");
            }
        }

        public async Task<UserModel> UpdateUser([FromForm] UpdateUserDto updateDto ,Guid id) {
            var user = await GetUserById(id);
            if (user == null)
            {
                throw new KeyNotFoundException("user does not match");
            }

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                user.Name = updateDto.Name;
            }

            if (!string.IsNullOrEmpty(updateDto.Username))
            {
                user.Username = updateDto.Username;
            }

            if (!string.IsNullOrEmpty(updateDto.Bio))
            {
                user.Bio = updateDto.Bio;
            }

            if (!string.IsNullOrEmpty(updateDto.Gender))
            {
                user.Gender = updateDto.Gender;
            }

            user.UpdatedDate = DateTime.UtcNow;

            if (updateDto.file != null)
            {
                var fileService = new FileService();
                var filePath = await fileService.SaveFileAsync(updateDto.file);
                user.ProfilePicture = filePath; 
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserModel> GetUserByEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }
            
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            return user;
        }


        private string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }
        
    }
}
