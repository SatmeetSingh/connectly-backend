using dating_app_backend.src.Models.Dto;
using dating_app_backend.src.Models.Entity;
using dating_app_backend.src.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UserService _userService { get; set; }
        public UsersController(UserService userService) {
            _userService = userService;
        }
       
        [HttpGet]
        public  async Task<IActionResult> GetAllUsers() {
            try {
                var users = await _userService.GetAllUsers();
                if (users.Count == 0)
                {
                    return NotFound("No Data Found");
                }
                return Ok(users);
            } catch(Exception ex) {
            return StatusCode(500, "An unexpected error occurred. "+ ex.Message);
            }
        }

        [EnableCors("AllowAll")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try {
                var user = await _userService.GetUserById(id);

                if (user == null){
                    return NotFound();  
                }
                return Ok(user);
            }
            catch (Exception ex){
                return StatusCode(500, "An unexpected error occurred. " + ex.Message);
            }
        }

        [EnableCors("AllowAll")]
        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser(SignUpUserDto userDto) {
            try {
                var newUser =  await _userService.SignUpUser(userDto);
                return Ok(new { message =  "User created Successfully"});
            } catch (DbUpdateException ex) {
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
                return Ok(new { message = "User logged in Successfully" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
    }
}
