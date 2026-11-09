using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Infrastructure;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RaceDayDbContext db, IPasswordService passwordService) : ControllerBase
{
    /// <summary>Registers a new Organiser or Participant account.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email)) return Conflict(new { message = "An account with that email already exists." });
        if (!request.Role.HasValue || !Enum.IsDefined(request.Role.Value)) return BadRequest(new { message = "Role must be Organiser or Participant." });

        var user = new User { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, Role = request.Role.Value, Phone = request.Phone?.Trim(), PasswordHash = passwordService.Hash(request.Password) };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        db.UserProfiles.Add(new UserProfile { UserId = user.UserId });
        await db.SaveChangesAsync();
        return Created($"/api/profile", new AuthResponse(user.UserId, user.FirstName, user.LastName, user.Email, user.Role.ToString()));
    }

    /// <summary>Logs a user in and creates a server-side session containing the authenticated user and role.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || !passwordService.Verify(request.Password, user.PasswordHash)) return Unauthorized(new { message = "Invalid email or password." });
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("Role", user.Role.ToString());
        return Ok(new AuthResponse(user.UserId, user.FirstName, user.LastName, user.Email, user.Role.ToString()));
    }

    /// <summary>Ends the current authenticated session.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return NoContent();
    }
}
