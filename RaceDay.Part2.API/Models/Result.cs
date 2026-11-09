namespace RaceDay.Models;

public enum ResultStatus { Finished, DNF, DNS, DSQ }

public class Result
{
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public ResultStatus ResultStatus { get; set; } = ResultStatus.Finished;
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public Enrolment Enrolment { get; set; } = null!;
}
