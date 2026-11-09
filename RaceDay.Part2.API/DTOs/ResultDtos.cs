using System.ComponentModel.DataAnnotations;
using RaceDay.Models;

namespace RaceDay.DTOs;

public class CreateResultRequest
{
    [Required] public int EnrolmentId { get; set; }
    public TimeSpan? FinishTime { get; set; }
    [Range(1, int.MaxValue)] public int? Position { get; set; }
    [Required] public ResultStatus ResultStatus { get; set; }
}

public class UpdateResultRequest
{
    public TimeSpan? FinishTime { get; set; }
    [Range(1, int.MaxValue)] public int? Position { get; set; }
    [Required] public ResultStatus ResultStatus { get; set; }
}

public class ResultResponse
{
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public ResultStatus ResultStatus { get; set; }
}
