using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAPI.Data;
using StudentAPI.DTOs;
using StudentAPI.Exceptions;
using StudentAPI.Interfaces;
using StudentAPI.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Twilio.TwiML.Voice;
using static StudentAPI.DTOs.AuthDtos;
using static StudentAPI.Exceptions.AppExceptions;

namespace StudentAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IEmailService _email;
        private readonly ISmsService _sms;

        public AuthService(AppDbContext db, IConfiguration config, IEmailService email, ISmsService sms)
        {
            _db = db;
            _config = config;
            _email = email;
            _sms = sms;
        }
        public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim();
            if (await _db.Users.AnyAsync(u => u.Email == email))
                throw new ConflictException($"An account with {email} is already registered. Please sign in instead.");
            // The principal may already have this person on the class roster (Students table) before they
            // self-register a login. Catch that here too, otherwise this silently created a second,
            // duplicate roster row with the same email instead of surfacing an error to the register page.
            if (await _db.Students.AnyAsync(s => s.Email == email))
                throw new ConflictException($"A student record for {email} already exists on the roster. Contact the principal/admin to link or approve your account instead of registering again.");
            var user = new User
            {
                Name = dto.Name,
                Email = email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                // Registration is pending until an administrator approves it.
                Role = "Student",
                RequestedRole = dto.Role.Trim().Equals("Teacher", StringComparison.OrdinalIgnoreCase) ? "Teacher" : "Student",
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
            };
            _db.Users.Add(user);
            if (user.RequestedRole == "Student")
            {
                _db.Students.Add(new Student
                {
                    Name = user.Name, Email = user.Email, City = "Not provided",
                    Course = "Undeclared", GPA = 0, EnrollDate = DateTime.UtcNow,
                    IsActive = false
                });
            }
            await _db.SaveChangesAsync();

            await _email.SendWelcomeEmailAsync(user.Email, user.Name);
            await _sms.SendWelcomeSmsAsync(user.Phone, user.Name);

            return new TokenResponseDto { Email = user.Email, Name = user.Name, Role = user.RequestedRole, IsApproved = false };
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var identity = dto.Email.Trim();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == identity || u.Name == identity);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new BadRequestException("Invalid Email or password!");
            if (!user.IsApproved)
                throw new BadRequestException("Your account is awaiting administrator approval.");
            return GenerateToken(user);
        }


        private TokenResponseDto GenerateToken(User user)
        {
            var secretkey = _config[
                "JwtSettings:SecretKey"]!;
            var issuer = _config[
                "JwtSettings:Issuer"]!;
            var audience = _config[
                "JwtSettings:Audience"]!;
            var expiryDays = int.Parse(_config[
                 "JwtSettings:Expirydays"]!);

            var keyBytes = Encoding.UTF8.GetBytes(secretkey);
            var key = new SymmetricSecurityKey(keyBytes);
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim> {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "Student" : char.ToUpper(user.Role[0]) + user.Role[1..].ToLowerInvariant()),
            };

            var expiry = DateTime.UtcNow.AddDays(expiryDays);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: cred
                );
        
            return new TokenResponseDto{ 
                    Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                Email = user.Email,
                Name = user.Name,
                Role = string.IsNullOrWhiteSpace(user.Role) ? "Student" : char.ToUpper(user.Role[0]) + user.Role[1..].ToLowerInvariant(),
                Expiry = expiry
                , IsApproved = user.IsApproved
            };

    }


    }
}
