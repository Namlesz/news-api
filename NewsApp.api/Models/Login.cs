using System.ComponentModel.DataAnnotations;

namespace NewsApp.api.Models;

public record Login(
    [Required(ErrorMessage = "E-mail is required")]
    string Email,
    [Required(ErrorMessage = "Password is required")]
    string Password
);

public record PasswordChange(
    [EmailAddress]
    [Required(ErrorMessage = "E-mail is required")]
    string Email,
    [Required(ErrorMessage = "Password is required")]
    string Password,
    [Required(ErrorMessage = "Token is required")]
    string Token
);