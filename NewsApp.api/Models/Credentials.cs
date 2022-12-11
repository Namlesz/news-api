using System.ComponentModel.DataAnnotations;

namespace NewsApp.api.Models;

public record Login(
    [Required]
    string Email,
    [Required]
    string Password
);

public record PasswordChange(
    [EmailAddress]
    [Required]
    string Email,
    [Required]
    string Password,
    [Required]
    string Token
);