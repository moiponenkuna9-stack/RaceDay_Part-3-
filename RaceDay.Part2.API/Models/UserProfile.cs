namespace RaceDay.Models;

public class UserProfile
{
    public int ProfileId { get; set; }
    public int UserId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Address { get; set; }
    public User User { get; set; } = null!;
}
