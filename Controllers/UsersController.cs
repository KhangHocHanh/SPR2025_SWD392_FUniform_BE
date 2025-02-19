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
    [Route("api")]
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
        [Route("Users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userRepo.GetUsers());
        }

        [Authorize]
        [HttpGet]
        [Route("Users/{id}")]
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

            if (currentUserRole == "staff" && user.Role.RoleName.ToLower() == "member")
            {
                // Staff can see only member
                return Ok(new { User = userDto });
            }

            return Forbid();
        }

        
        [HttpPut]
        [Route("Users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO user)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var userUpdate = await _userRepo.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            //if (currentUserRole == "admin" || currentUserId == id)
           // {
                userUpdate.FullName = user.FullName;
                userUpdate.Gender = user.Gender;
                userUpdate.DateOfBirth = user.DateOfBirth;
                userUpdate.Address = user.Address;
                userUpdate.Phone = user.Phone;
                userUpdate.Email = user.Email;
                userUpdate.Avatar = user.Avatar;


                await _userRepo.UpdateUser(userUpdate);
                return Ok(new { Message = "User updated successfully." });
           // }

           // return Forbid();
        }

        [HttpPut]
        [Route("Users/{id}/Password")]
        public async Task<IActionResult> ChangePassword(int id, [FromQuery] ChangePasswordDTO changePasswordDto)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var user = await _userRepo.GetUserById(id);
            if (user == null) return NotFound(new { Message = "User not found" });

           // if (currentUserRole == "admin" || currentUserId == id)
          //  {
                if (user.Password != changePasswordDto.OldPassword)
                {
                    return BadRequest(new { Message = "Old password is incorrect." });
                }

                user.Password = changePasswordDto.NewPassword;
                await _userRepo.UpdateUser(user);
                return Ok(new { Message = "✅ Password changed successfully." });
         //   }
         //   return Forbid();
        }

        
        [HttpDelete]
        [Route("Users/{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null) return NotFound(new { Message = "User not found" });
            if (user.IsDeleted == true) return BadRequest(new { Message = "User has been deleted" });

            user.IsDeleted = true;
            await _userRepo.UpdateUser(user);
            return Ok(new { Message = "User deactivated successfully." });
        }

        [HttpPut]
        [Route("Users/{id}/Recovery")]
        public async Task<IActionResult> RecoverUser(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null) return NotFound(new { Message = "User not found" });
            if (user.IsDeleted == false) return BadRequest(new { Message = "User has been existing" });

            user.IsDeleted = false;
            await _userRepo.UpdateUser(user);
            return Ok(new { Message = "User recovered successfully." });
        }

        #endregion 


    }
}
