using Microsoft.AspNetCore.Mvc;
using StudentAPI.DTOs;
using StudentAPI.Interfaces;

using static StudentAPI.DTOs.AuthDtos;

namespace StudentAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController :ControllerBase
    {

        private readonly IAuthService _auth;
        private readonly IEmailService _emailService;
        public AuthController(IEmailService emailService,  IAuthService auth)
        {
            _emailService = emailService;
            _auth = auth;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _auth.RegisterAsync(dto);
            //return Ok(token);
           

            return Ok(token);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto) 
        {
            var token = await _auth.LoginAsync(dto);
            return Ok(token);
        }
    }
}
