using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Infrastructure;

namespace RaceDay.Controllers;

[ApiController]
[Route("api")]
public class CategoriesController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Lists all reusable race categories.</summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll() => Ok(await db.Categories.AsNoTracking().OrderBy(c => c.DistanceKm).Select(c => new CategoryResponse { CategoryId = c.CategoryId, Name = c.Name, DistanceKm = c.DistanceKm, MaxParticipants = c.MaxParticipants }).ToListAsync());

    /// <summary>Adds an existing category to an organiser-owned event.</summary>
    [HttpPost("events/{eventId:int}/categories")]
    [SessionAuthorize("Organiser")]
    public async Task<ActionResult> Add(int eventId, AddCategoryRequest request)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var eventItem = await db.Events.SingleOrDefaultAsync(e => e.EventId == eventId);
        if (eventItem is null) return NotFound(new { message = "Event not found." });
        if (eventItem.OrganiserId != organiserId) return Forbid();
        if (!await db.Categories.AnyAsync(c => c.CategoryId == request.CategoryId)) return NotFound(new { message = "Category not found." });
        if (await db.EventCategories.AnyAsync(ec => ec.EventId == eventId && ec.CategoryId == request.CategoryId)) return Conflict(new { message = "Category is already assigned to this event." });
        db.EventCategories.Add(new RaceDay.Models.EventCategory { EventId = eventId, CategoryId = request.CategoryId, EntryFee = request.EntryFee });
        await db.SaveChangesAsync();
        return Created($"/api/events/{eventId}", new { eventId, request.CategoryId, request.EntryFee });
    }

    /// <summary>Removes an event category when no participant has enrolled in it.</summary>
    [HttpDelete("events/{eventId:int}/categories/{categoryId:int}")]
    [SessionAuthorize("Organiser")]
    public async Task<IActionResult> Remove(int eventId, int categoryId)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var link = await db.EventCategories.Include(x => x.Event).SingleOrDefaultAsync(x => x.EventId == eventId && x.CategoryId == categoryId);
        if (link is null) return NotFound();
        if (link.Event.OrganiserId != organiserId) return Forbid();
        if (await db.Enrolments.AnyAsync(e => e.EventId == eventId && e.CategoryId == categoryId && e.Status != RaceDay.Models.EnrolmentStatus.Cancelled)) return Conflict(new { message = "Category cannot be removed after enrolments exist." });
        db.EventCategories.Remove(link); await db.SaveChangesAsync(); return NoContent();
    }
}
