using System.ComponentModel.DataAnnotations;
using RaceDay.Models;

namespace RaceDay.DTOs;

public class RegisterRequest
{
    [Required, StringLength(50)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(50)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
    [Required] public UserRole? Role { get; set; }
    public string? Phone { get; set; }
}

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public record AuthResponse(int UserId, string FirstName, string LastName, string Email, string Role);
