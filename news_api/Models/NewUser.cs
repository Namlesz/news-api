using System.ComponentModel.DataAnnotations;

namespace news_api.Models;

public class NewUser
{
    [Required(ErrorMessage = "Imię jest wymagane")]
    public string Name { get; init; } = null!;

    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    public string Surname { get; init; } = null!;

    [Required(ErrorMessage = "Nick jest wymagany")]
    public string Username { get; init; } = null!;

    [EmailAddress]
    [Required(ErrorMessage = "Email jest wymagany")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    public string Password { get; init; } = null!;
}