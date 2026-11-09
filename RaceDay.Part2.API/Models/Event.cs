namespace RaceDay.Models;

public enum EventStatus { Draft, Open, Closed, Completed, Cancelled }
public enum EventType { Run, Walk, Cycle }

public class Event
{
    public int EventId { get; set; }
    public int OrganiserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public EventType EventType { get; set; }
    public DateTime RegistrationOpen { get; set; }
    public DateTime RegistrationClose { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Organiser { get; set; } = null!;
    public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
