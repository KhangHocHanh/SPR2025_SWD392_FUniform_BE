using ClothingCustomization.Data;
using ClothingCustomization.DTO;
using ClothingCustomization.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ClothingCustomization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepository userRepo, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId",user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.RoleName.ToLower())  // Store Role in Token
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signIn
                    );

                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                // map to watch information
                var userDto = new 
                {
                    Username = user.Username,
                    Password = user.Password,
                    FullName = user.FullName,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    IsDeleted = user.IsDeleted,
                };
                return Ok(new { Token = tokenValue, User = userDto } );
                //return Ok(new { Role = user.Role?.RoleName, Message = "Debugging Role Value" });
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

        [Authorize(Roles = "admin")]
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
