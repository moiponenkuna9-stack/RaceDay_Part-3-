namespace RaceDay.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public int MaxParticipants { get; set; }
    public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
}

public class EventCategory
{
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public decimal EntryFee { get; set; }
    public Event Event { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
