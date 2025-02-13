using ClothingCustomization.Data;
using ClothingCustomization.DTO;
using ClothingCustomization.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ClothingCustomization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUserRepository userRepo, IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromQuery] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user already exists
            var existingUser = (await _userRepo.GetUsers()).FirstOrDefault(x => x.Username == userDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Tai khoan nay da su dung");
            }


            // Map DTO to Entity
            var newUser = new User
            {
                Username = userDto.Username,
                Password = userDto.Password,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                DateOfBirth = userDto.DateOfBirth,
                Address = userDto.Address,
                Phone = userDto.Phone,
                Email = userDto.Email,
                RoleId = 3,
                IsDeleted = false,
            };

            await _userRepo.Register(newUser);

            return Ok(new { Message = "Account register successfully", UserId = newUser.UserId });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string taikhoan, string matkhau)
        {
            var user = await _userRepo.Login(taikhoan, matkhau);
            if (user != null)
            {
                // Store user in session
                _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.UserId);
                return Ok(user);
            }
            return NoContent();
        }

        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return BadRequest(new { Message = "You are not login" });
            }

            _httpContextAccessor.HttpContext.Session.Clear();
            return Ok(new { Message = "Logout successfully" });
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userRepo.GetUsers());
        }

        [HttpGet]
        [Route("GetUser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user != null)
            {
                return Ok(user);
            }
            return NoContent();
        }
    }
}
