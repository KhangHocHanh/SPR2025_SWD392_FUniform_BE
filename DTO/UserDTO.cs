using System.ComponentModel.DataAnnotations;

namespace ClothingCustomization.DTO
{
    public class UserDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        [DataType("Datetime")]
        public DateTime DateOfBirth { get; set; }
        
        public string? Address { get; set; }

        public string? Phone { get; set; }
        [Required]
        public string Email { get; set; }
  
        public string? Avatar { get; set; }

    }
}
