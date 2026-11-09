using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Infrastructure;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api")]
public class ResultsController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Returns published results for an event, ordered by finishing position.</summary>
    [HttpGet("events/{eventId:int}/results")]
    public async Task<ActionResult<IEnumerable<ResultResponse>>> GetEventResults(int eventId)
    {
        var exists = await db.Events.AnyAsync(e => e.EventId == eventId);
        if (!exists) return NotFound();
        return Ok(await Query().Where(r => r.EventId == eventId).OrderBy(r => r.Position).ThenBy(r => r.ResultId).ToListAsync());
    }

    /// <summary>Records a result for a participant enrolment belonging to an organiser's event.</summary>
    [HttpPost("events/{eventId:int}/results")]
    [SessionAuthorize("Organiser")]
    public async Task<ActionResult<ResultResponse>> Create(int eventId, CreateResultRequest request)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var enrolment = await db.Enrolments.Include(e => e.Event).SingleOrDefaultAsync(e => e.EnrolmentId == request.EnrolmentId && e.EventId == eventId);
        if (enrolment is null) return NotFound(new { message = "Enrolment not found for this event." });
        if (enrolment.Event.OrganiserId != organiserId) return Forbid();
        if (await db.Results.AnyAsync(r => r.EnrolmentId == request.EnrolmentId)) return Conflict(new { message = "A result already exists for this enrolment." });
        var result = new Result { EnrolmentId = request.EnrolmentId, FinishTime = request.FinishTime, Position = request.Position, ResultStatus = request.ResultStatus };
        db.Results.Add(result); enrolment.Status = EnrolmentStatus.Completed; await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = result.ResultId }, await Query().SingleAsync(r => r.ResultId == result.ResultId));
    }

    /// <summary>Corrects a result for an organiser-owned event.</summary>
    [HttpPut("results/{id:int}")]
    [SessionAuthorize("Organiser")]
    public async Task<ActionResult<ResultResponse>> Update(int id, UpdateResultRequest request)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var result = await db.Results.Include(r => r.Enrolment).ThenInclude(e => e.Event).SingleOrDefaultAsync(r => r.ResultId == id);
        if (result is null) return NotFound();
        if (result.Enrolment.Event.OrganiserId != organiserId) return Forbid();
        result.FinishTime = request.FinishTime; result.Position = request.Position; result.ResultStatus = request.ResultStatus; await db.SaveChangesAsync();
        return Ok(await Query().SingleAsync(r => r.ResultId == id));
    }

    /// <summary>Returns one result. Participants may view only their own result.</summary>
    [HttpGet("results/{id:int}")]
    public async Task<ActionResult<ResultResponse>> Get(int id)
    {
        var result = await Query().SingleOrDefaultAsync(r => r.ResultId == id);
        if (result is null) return NotFound();
        var role = HttpContext.Session.GetString("Role");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (role == nameof(UserRole.Participant) && result.ParticipantId != userId) return Forbid();
        if (role == null) return Ok(result); // published result endpoint is public
        return Ok(result);
    }

    private IQueryable<ResultResponse> Query() => db.Results.AsNoTracking().Select(r => new ResultResponse
    {
        ResultId = r.ResultId, EnrolmentId = r.EnrolmentId, ParticipantId = r.Enrolment.ParticipantId,
        ParticipantName = r.Enrolment.Participant.FirstName + " " + r.Enrolment.Participant.LastName,
        EventId = r.Enrolment.EventId, EventName = r.Enrolment.Event.Name, FinishTime = r.FinishTime, Position = r.Position, ResultStatus = r.ResultStatus
    });
}
