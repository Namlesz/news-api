using System.ComponentModel.DataAnnotations;

namespace news_api.Models;

public class Login
{
    [Required(ErrorMessage = "E-mail is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}