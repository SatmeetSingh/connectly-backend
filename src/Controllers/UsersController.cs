using Asp.Versioning;
using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace dating_app_backend.src.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private UserService _userService { get; set; }
        public UsersController(UserService userService , ILogger<UsersController> logger) {
            _userService = userService;
            _logger = logger;
        }
       
        [HttpGet]
        public  async Task<IActionResult> GetAllUsers() {
            _logger.LogInformation("Fetching all users in database");
            try
            {
                var users = await _userService.GetAllUsers();
                if (users.Count == 0)
                {
                    _logger.LogWarning("Users not found in database");
                    return NotFound("Users not found");
                }

                _logger.LogInformation("Successfully fetched users ");
                return Ok(new { users =  users, count = users.Count});
            } catch(Exception ex) {
                _logger.LogError(ex, "An error occurred while fetching user");
                return StatusCode(500, "An unexpected error occurred. "+ ex.Message);
            }
        }

        [EnableCors("AllowAll")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try {
                var user = await _userService.GetUserById(id);

                if (user == null) {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex) {
                return StatusCode(500, "An unexpected error occurred. " + ex.Message);
            }
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult>  GetUserProfile(Guid id)
        {
            try
            {
                var user = await _userService.GetUserProfile(id);

                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. " + ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUser(string username) {
            if(username.Length == 0)
            {
                return Ok();
            }
            try { 
            var users = await _userService.SearchUserByUsername(username);
                if (users.Count == 0)
                {
                    return NotFound("No users found matching the username.");
                }
                return Ok(users);
            } catch (Exception ex) {
                return StatusCode(500, "An unexpected error occurred. " + ex.Message);
            }
        }

        [EnableCors("AllowAll")]
        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser(SignUpUserDto userDto) {
            try 
            {
                var newUser =  await _userService.SignUpUser(userDto);
                return Ok(new { message =  "User created Successfully"});
            }
            catch (DbUpdateException ex) 
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [EnableCors("AllowAll")]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto userDto)
        {
            try
            {
                var newUser = await _userService.LoginUser(userDto);
                return Ok(new { message = "User logged in Successfully" , user = newUser  });
            }
            catch (KeyNotFoundException) {
                     return NotFound();
            }
            catch (UnauthorizedAccessException )
            {
                return Unauthorized(new Hashtable()  {
                    { "status" , 401 },
                    { "message", "Incorrect password. Please try again." }
                });
            }
        }
        
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Updateuser([FromForm] UpdateUserDto updateDto , Guid id) {
            try
            {
                var User = await _userService.UpdateUser(updateDto , id);
                if (id ==    Guid.Empty)
                {
                    return BadRequest(new { error = "Invalid or missing ID." });
                }   
                if(User == null)
                {
                    return NotFound(new { error = "User with the specified ID was not found." });
                }
                return Ok(new { message = "User logged in Successfully", user = User });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request. Please try again later." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try {
                await _userService.DeleteUser(id);
                return Ok(new {message = "User amd all related posts Deleted successfully"});            
            } catch(Exception) {
                return StatusCode(500, new { error = "An error occurred while processing your request. Please try again later." });

            }
        }
    }
}       
