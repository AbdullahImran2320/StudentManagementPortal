using System.ComponentModel.DataAnnotations;

namespace StudentAPI.DTOs
{
    public class AuthDtos
    {
        public class LoginDto
        {
            [Required]
            public string Email { get; set; }= string.Empty;    
            [Required]
            [MinLength(6)]
            public string Password { get; set; }= string.Empty;

        }

        public class RegisterDto
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = "Student";
            [Required]
            [Phone]
            public string Phone { get; set; } = string.Empty;

        }

        public class TokenResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
            public bool IsApproved { get; set; }

        }



    }
}
