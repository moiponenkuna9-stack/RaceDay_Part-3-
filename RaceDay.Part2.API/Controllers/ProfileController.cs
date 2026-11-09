using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Infrastructure;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/profile")]
[SessionAuthorize]
public class ProfileController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Returns the authenticated user's own profile.</summary>
    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> Get()
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        var user = await db.Users.Include(u => u.Profile).SingleAsync(u => u.UserId == id);
        return Ok(ToResponse(user));
    }

    /// <summary>Updates the authenticated user's own profile and contact details.</summary>
    [HttpPut]
    public async Task<ActionResult<ProfileResponse>> Update(UpdateProfileRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        var user = await db.Users.Include(u => u.Profile).SingleAsync(u => u.UserId == id);
        user.FirstName = request.FirstName.Trim(); user.LastName = request.LastName.Trim(); user.Phone = request.Phone?.Trim();
        user.Profile ??= new Models.UserProfile { UserId = id };
        user.Profile.DateOfBirth = request.DateOfBirth; user.Profile.Gender = request.Gender; user.Profile.EmergencyContactName = request.EmergencyContactName; user.Profile.EmergencyContactPhone = request.EmergencyContactPhone; user.Profile.Address = request.Address;
        await db.SaveChangesAsync();
        return Ok(ToResponse(user));
    }

    private static ProfileResponse ToResponse(Models.User user) => new()
    {
        UserId = user.UserId, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Role = user.Role.ToString(), Phone = user.Phone,
        DateOfBirth = user.Profile?.DateOfBirth, Gender = user.Profile?.Gender, EmergencyContactName = user.Profile?.EmergencyContactName, EmergencyContactPhone = user.Profile?.EmergencyContactPhone, Address = user.Profile?.Address
    };
}
