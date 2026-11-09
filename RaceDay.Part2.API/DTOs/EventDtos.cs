using System.ComponentModel.DataAnnotations;
using RaceDay.Models;

namespace RaceDay.DTOs;

public class CreateEventRequest
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(500)] public string? Description { get; set; }
    [Required] public DateTime EventDate { get; set; }
    [Required, StringLength(200)] public string Location { get; set; } = string.Empty;
    [Range(0.1, 500)] public decimal DistanceKm { get; set; }
    [Required] public EventType EventType { get; set; }
    [Required] public DateTime RegistrationOpen { get; set; }
    [Required] public DateTime RegistrationClose { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Open;
}

public class EventResponse
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
    public EventStatus Status { get; set; }
    public List<CategoryResponse> Categories { get; set; } = new();
}

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public int MaxParticipants { get; set; }
    public decimal? EntryFee { get; set; }
}

public class AddCategoryRequest
{
    [Required] public int CategoryId { get; set; }
    [Range(0, 100000)] public decimal EntryFee { get; set; }
}
