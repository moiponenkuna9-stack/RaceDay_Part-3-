namespace RaceDay.Models;

public enum EnrolmentStatus { Active, Cancelled, Completed }

public class Enrolment
{
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
    public EnrolmentStatus Status { get; set; } = EnrolmentStatus.Active;
    public int RaceNumber { get; set; }

    public User Participant { get; set; } = null!;
    public Event Event { get; set; } = null!;
    public EventCategory EventCategory { get; set; } = null!;
    public Result? Result { get; set; }
}
