using dating_app_backend.src.DB;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading;


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
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public async Task<(List<UserDto> users, int totalCount)> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken)
        {
            int totalCount = await _context.Users
                            .AsNoTracking()
                            .Select(u => 1)  // Optimized Count
                            .FirstOrDefaultAsync(cancellationToken);
            if (totalCount == 0)
            {
                return (new List<UserDto>(), 0);
            }

            var users = await _context.Users.OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture,
                    Bio = u.Bio,
                    Gender = u.Gender,
                    FollowersCount = u.FollowersCount,
                    FollowingCount = u.FollowingCount,
                    PostCount = u.PostCount,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate,
                    UpdatedDate = u.UpdatedDate
                }).OrderByDescending(p => p.CreatedDate).ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        public async Task<UserDto> GetUserProfile(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            }
            else
            {
                var user = await _context.Users.Where(e => e.Id == id).Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture,
                    Bio = u.Bio,
                    Gender = u.Gender,
                    FollowersCount = u.FollowersCount,
                    FollowingCount = u.FollowingCount,
                    PostCount = u.PostCount,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate,
                    UpdatedDate = u.UpdatedDate
                }).FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }
                return user;
            }
        }

        public async Task<UserModel> GetUserById(Guid id) {
            if (id == Guid.Empty)
            {
                throw new BadHttpRequestException("Id cannot be empty (Bad Request)", 400);
            } else
            {
                 var user  =  await _context.Users.FirstOrDefaultAsync(e => e.Id == id);
                if(user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }
                return user;
            }
        }

        public async Task<List<UserModel>> SearchUserByUsername(string username)
        {
            username = username.Trim().ToLower();

            var usersByName =await _context.Users.Where(u => u.Username.StartsWith(username)).Take(7).ToListAsync();
            return usersByName;
        }

        public async Task<UserModel> SignUpUser(SignUpUserDto userDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    bool userExists = await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Username == userDto.Username);
                    if (userExists)
                    {
                        throw new KeyNotFoundException("A user with the same email or same username already exists.");
                    }

                    var user = new UserModel
                    {
                        Username = userDto.Username,
                        Name = userDto.Name,
                        Email = userDto.Email,
                        Password = HashPassword(userDto.Password)
                    };
                   

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return user;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

        }

        public async Task<UserModel> LoginUser([FromBody] LoginDto loginDto)
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

        public async Task DeleteUser(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Likes.Where(l => l.UserId == id).ExecuteDeleteAsync();     

                await _context.Comments.Where(c => c.UserId == id).ExecuteDeleteAsync();  

                await _context.Follows.Where(f => f.FollowerId == id || f.FolloweeId == id).ExecuteDeleteAsync();

                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<UserModel> GetUserByEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }
            
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new KeyNotFoundException($"No user found with email {email}");
            }
            return user;
        }


        private static string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password,10);   // 10 salt
            return hashedPassword;
        }
        
    }
}
