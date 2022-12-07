using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace NewsApp.api.Models;

[BsonIgnoreExtraElements]
[CollectionName("Users")]
public class UserInfo
{
    [BsonId] [BsonElement("_id")] public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? EditorialOfficeId { get; set; }
}

public record NewUser
(
    [Required(ErrorMessage = "Imię jest wymagane")]
    string Name,
    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    string Surname,
    [EmailAddress]
    [Required(ErrorMessage = "Email jest wymagany")]
    string Email,
    [Required(ErrorMessage = "Hasło jest wymagane")]
    string Password
);