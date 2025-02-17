using ClothingCustomization.Data;
using ClothingCustomization.DTO;
using ClothingCustomization.Repository;
using ClothingCustomization.Services;
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
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;
        public UsersController(IUserRepository userRepo, IConfiguration configuration, JwtService jwtService)
        {
            _userRepo = userRepo;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        #region Authentication & Authorization
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
                return BadRequest("This account name has been used");
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
                string tokenValue = _jwtService.GenerateToken(user.UserId, user.Role.RoleName);

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
            return Ok(new { Message = "Logout successfully" });
        }

        #endregion


        #region User Management
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userRepo.GetUsers());
        }

        [Authorize]
        [HttpGet]
        [Route("GetUsers/{id}")]
        public async Task<IActionResult> GetUsers(int id)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var user = await _userRepo.GetUserById(id);

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
            if (user == null)
            {
                return NoContent();
            }

            if (currentUserRole == "admin" || currentUserId == id)
            {
                // Admins can see all, users can see their own account
                return Ok(new { User = userDto });
            }

            if (currentUserRole == "staff" && user.Role.RoleName.ToLower() != "admin")
            {
                // Staff can see everyone EXCEPT admins
                return Ok(new { User = userDto });
            }

            return Forbid();
        }
        #endregion 


    }
}
