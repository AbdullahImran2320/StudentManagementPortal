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
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                throw new ConflictException($"Email {dto.Email} already registered!");
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await _email.SendWelcomeEmailAsync(user.Email, user.Name);
            await _sms.SendWelcomeSmsAsync(user.Phone, user.Name);

            return GenerateToken(user);
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new BadRequestException("Invalid Email or password!");
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
                new Claim(ClaimTypes.Role, user.Role),
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
                Role = user.Role,
                Expiry = expiry
            };

    }


    }
}