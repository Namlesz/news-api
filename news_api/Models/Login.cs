using System.ComponentModel.DataAnnotations;

namespace news_api.Models;

public record Login(
    [Required(ErrorMessage = "E-mail is required")]
    string Email,
    [Required(ErrorMessage = "Password is required")]
    string Password
);