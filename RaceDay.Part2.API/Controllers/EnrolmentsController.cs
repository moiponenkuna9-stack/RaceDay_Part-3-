using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Infrastructure;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api")]
public class EnrolmentsController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Enrols the logged-in participant in a category for an event.</summary>
    [HttpPost("events/{eventId:int}/enrolments")]
    [SessionAuthorize("Participant")]
    public async Task<ActionResult<EnrolmentResponse>> Create(int eventId, CreateEnrolmentRequest request)
    {
        var participantId = HttpContext.Session.GetInt32("UserId")!.Value;
        var item = await db.Events.Include(e => e.EventCategories).SingleOrDefaultAsync(e => e.EventId == eventId);
        if (item is null) return NotFound(new { message = "Event not found." });
        if (item.Status != EventStatus.Open || DateTime.UtcNow < item.RegistrationOpen || DateTime.UtcNow > item.RegistrationClose) return Conflict(new { message = "Registration is closed for this event." });
        var link = item.EventCategories.SingleOrDefault(x => x.CategoryId == request.CategoryId);
        if (link is null) return BadRequest(new { message = "The selected category is not available for this event." });
        if (await db.Enrolments.AnyAsync(e => e.ParticipantId == participantId && e.EventId == eventId && e.Status != EnrolmentStatus.Cancelled)) return Conflict(new { message = "Participant is already enrolled in this event." });
        var count = await db.Enrolments.CountAsync(e => e.EventId == eventId && e.CategoryId == request.CategoryId && e.Status != EnrolmentStatus.Cancelled);
        var category = await db.Categories.FindAsync(request.CategoryId);
        if (category is null) return NotFound();
        if (count >= category.MaxParticipants) return Conflict(new { message = "The selected category is full." });
        var raceNumber = (await db.Enrolments.Where(e => e.EventId == eventId).Select(e => (int?)e.RaceNumber).MaxAsync() ?? 100) + 1;
        var enrolment = new Enrolment { ParticipantId = participantId, EventId = eventId, CategoryId = request.CategoryId, RaceNumber = raceNumber, Status = EnrolmentStatus.Active };
        db.Enrolments.Add(enrolment); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMine), new { }, await ToResponse(enrolment.EnrolmentId));
    }

    /// <summary>Returns the authenticated participant's enrolments.</summary>
    [HttpGet("enrolments")]
    [SessionAuthorize("Participant")]
    public async Task<ActionResult<IEnumerable<EnrolmentResponse>>> GetMine()
    {
        var participantId = HttpContext.Session.GetInt32("UserId")!.Value;
        var items = await db.Enrolments.Where(e => e.ParticipantId == participantId).OrderByDescending(e => e.EnrolmentDate).ToListAsync();
        return Ok(await ToResponses(items));
    }

    /// <summary>Returns all enrolments for an event owned by the logged-in organiser.</summary>
    [HttpGet("events/{eventId:int}/enrolments")]
    [SessionAuthorize("Organiser")]
    public async Task<ActionResult<IEnumerable<EnrolmentResponse>>> GetForEvent(int eventId)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        if (!await db.Events.AnyAsync(e => e.EventId == eventId && e.OrganiserId == organiserId)) return Forbid();
        var items = await db.Enrolments.Where(e => e.EventId == eventId).OrderBy(e => e.RaceNumber).ToListAsync();
        return Ok(await ToResponses(items));
    }

    /// <summary>Cancels the authenticated participant's enrolment before the registration deadline.</summary>
    [HttpDelete("enrolments/{id:int}")]
    [SessionAuthorize("Participant")]
    public async Task<IActionResult> Cancel(int id)
    {
        var participantId = HttpContext.Session.GetInt32("UserId")!.Value;
        var item = await db.Enrolments.Include(e => e.Event).SingleOrDefaultAsync(e => e.EnrolmentId == id && e.ParticipantId == participantId);
        if (item is null) return NotFound();
        if (DateTime.UtcNow > item.Event.RegistrationClose) return Conflict(new { message = "The enrolment cancellation deadline has passed." });
        item.Status = EnrolmentStatus.Cancelled; await db.SaveChangesAsync(); return NoContent();
    }

    private async Task<EnrolmentResponse> ToResponse(int id) => await ToResponseQuery().SingleAsync(e => e.EnrolmentId == id);
    private async Task<List<EnrolmentResponse>> ToResponses(List<Enrolment> items) { var ids = items.Select(x => x.EnrolmentId).ToList(); return await ToResponseQuery().Where(x => ids.Contains(x.EnrolmentId)).ToListAsync(); }
    private IQueryable<EnrolmentResponse> ToResponseQuery() => db.Enrolments.AsNoTracking().Select(e => new EnrolmentResponse { EnrolmentId = e.EnrolmentId, ParticipantId = e.ParticipantId, ParticipantName = e.Participant.FirstName + " " + e.Participant.LastName, EventId = e.EventId, EventName = e.Event.Name, CategoryId = e.CategoryId, CategoryName = e.EventCategory.Category.Name, EnrolmentDate = e.EnrolmentDate, Status = e.Status.ToString(), RaceNumber = e.RaceNumber });
}
