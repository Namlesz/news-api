using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace news_api.Models;

[BsonIgnoreExtraElements]
[CollectionName("Users")]
public class UserInfo
{
    [BsonId] [BsonElement("_id")] public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    [BsonElement("UserName")] public string? Username { get; set; }
    public string? Email { get; set; }
}

public record NewUser
(
    [Required(ErrorMessage = "Imię jest wymagane")] string Name,
    [Required(ErrorMessage = "Nazwisko jest wymagane")] string Surname,
    [Required(ErrorMessage = "Nick jest wymagany")] string Username,
    [EmailAddress][Required(ErrorMessage = "Email jest wymagany")] string Email,
    [Required(ErrorMessage = "Hasło jest wymagane")] string Password
);