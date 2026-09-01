using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Controllers;

public record CreateStaffDto([Required] string Name, [Required, EmailAddress] string Email, [Required, MinLength(6)] string Password, [Required] string Role, string Phone = "");
public record UpdateAccountDto(bool? IsApproved, string? Role);

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin,admin")]
public class UserManagementController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Users.AsNoTracking().OrderBy(x => x.IsApproved).ThenBy(x => x.Name).Select(x => new { x.Id, x.Name, x.Email, x.Phone, x.Role, x.RequestedRole, x.IsApproved, x.CreatedAt }).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateStaffDto dto)
    {
        var role = dto.Role.Trim().ToLowerInvariant();
        if (role is not ("teacher" or "admin")) return BadRequest("Only Teacher or Admin staff accounts can be created here.");
        if (await db.Users.AnyAsync(x => x.Email == dto.Email)) return Conflict("An account with this email already exists.");
        var roleName = char.ToUpper(role[0]) + role[1..];
        var user = new User { Name = dto.Name.Trim(), Email = dto.Email.Trim(), Phone = dto.Phone.Trim(), Role = roleName, RequestedRole = roleName, IsApproved = true, PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), CreatedAt = DateTime.UtcNow };
        db.Users.Add(user); await db.SaveChangesAsync();
        return Created($"api/admin/users/{user.Id}", new { user.Id, user.Name, user.Email, user.Role });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAccount(int id, UpdateAccountDto dto)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound("Account not found.");
        if (dto.IsApproved is not null) user.IsApproved = dto.IsApproved.Value;
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            var role = dto.Role.Trim().ToLowerInvariant();
            if (role is not ("student" or "teacher" or "admin")) return BadRequest("Role must be Student, Teacher, or Admin.");
            user.Role = char.ToUpper(role[0]) + role[1..];
            user.RequestedRole = user.Role;
        }
        var linkedStudent = await db.Students.FirstOrDefaultAsync(s => s.Email == user.Email);
        if (linkedStudent is not null) linkedStudent.IsActive = user.IsApproved && string.Equals(user.Role, "Student", StringComparison.OrdinalIgnoreCase);
        await db.SaveChangesAsync();
        return Ok(new { user.Id, user.Name, user.Email, user.Role, user.RequestedRole, user.IsApproved });
    }
}
