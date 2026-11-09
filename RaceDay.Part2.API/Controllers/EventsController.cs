using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Infrastructure;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Lists events. Optional status filtering is supported.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetAll([FromQuery] EventStatus? status = null)
    {
        var query = db.Events.Include(e => e.EventCategories).ThenInclude(ec => ec.Category).AsNoTracking();
        if (status.HasValue) query = query.Where(e => e.Status == status.Value).Include(e => e.EventCategories).ThenInclude(ec => ec.Category);
        var events = await query.OrderBy(e => e.EventDate).ToListAsync();
        return Ok(events.Select(ToResponse));
    }

    /// <summary>Returns one event and its available categories.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponse>> Get(int id)
    {
        var item = await db.Events.Include(e => e.EventCategories).ThenInclude(ec => ec.Category).AsNoTracking().SingleOrDefaultAsync(e => e.EventId == id);
        return item is null ? NotFound() : Ok(ToResponse(item));
    }

    /// <summary>Creates an event for the logged-in organiser.</summary>
    [HttpPost]
    [SessionAuthorize("Organiser")]
    public async Task<ActionResult<EventResponse>> Create(CreateEventRequest request)
    {
        if (request.RegistrationClose < request.RegistrationOpen || request.EventDate.Date < request.RegistrationClose.Date) return BadRequest(new { message = "Registration dates and event date are invalid." });
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var item = new Event { OrganiserId = organiserId, Name = request.Name.Trim(), Description = request.Description, EventDate = request.EventDate, Location = request.Location.Trim(), DistanceKm = request.DistanceKm, EventType = request.EventType, RegistrationOpen = request.RegistrationOpen, RegistrationClose = request.RegistrationClose, Status = request.Status };
        db.Events.Add(item); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.EventId }, ToResponse(item));
    }

    /// <summary>Updates an event owned by the logged-in organiser.</summary>
    [HttpPut("{id:int}")]
    [SessionAuthorize("Organiser")]
    public async Task<ActionResult<EventResponse>> Update(int id, CreateEventRequest request)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var item = await db.Events.Include(e => e.EventCategories).ThenInclude(ec => ec.Category).SingleOrDefaultAsync(e => e.EventId == id);
        if (item is null) return NotFound();
        if (item.OrganiserId != organiserId) return Forbid();
        if (request.RegistrationClose < request.RegistrationOpen || request.EventDate.Date < request.RegistrationClose.Date) return BadRequest(new { message = "Registration dates and event date are invalid." });
        item.Name = request.Name.Trim(); item.Description = request.Description; item.EventDate = request.EventDate; item.Location = request.Location.Trim(); item.DistanceKm = request.DistanceKm; item.EventType = request.EventType; item.RegistrationOpen = request.RegistrationOpen; item.RegistrationClose = request.RegistrationClose; item.Status = request.Status;
        await db.SaveChangesAsync(); return Ok(ToResponse(item));
    }

    /// <summary>Deletes an organiser-owned event that has no enrolments and has not started.</summary>
    [HttpDelete("{id:int}")]
    [SessionAuthorize("Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var item = await db.Events.Include(e => e.Enrolments).SingleOrDefaultAsync(e => e.EventId == id);
        if (item is null) return NotFound();
        if (item.OrganiserId != organiserId) return Forbid();
        if (item.EventDate <= DateTime.UtcNow || item.Enrolments.Count > 0) return Conflict(new { message = "An event that has started or has enrolments cannot be deleted." });
        db.Events.Remove(item); await db.SaveChangesAsync(); return NoContent();
    }

    private static EventResponse ToResponse(Event e) => new()
    {
        EventId = e.EventId, OrganiserId = e.OrganiserId, Name = e.Name, Description = e.Description, EventDate = e.EventDate, Location = e.Location, DistanceKm = e.DistanceKm, EventType = e.EventType, RegistrationOpen = e.RegistrationOpen, RegistrationClose = e.RegistrationClose, Status = e.Status,
        Categories = e.EventCategories.Select(ec => new CategoryResponse { CategoryId = ec.CategoryId, Name = ec.Category.Name, DistanceKm = ec.Category.DistanceKm, MaxParticipants = ec.Category.MaxParticipants, EntryFee = ec.EntryFee }).ToList()
    };
}
