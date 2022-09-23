using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using news_api.Interfaces;

namespace news_api.Models;

[BsonIgnoreExtraElements]
[CollectionName("Users")]
public class User : IUser
{
    [BsonId] 
    [BsonElement("_id")] 
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    public string Surname { get; set; } = null!;

    [BsonElement("UserName")]
    [Required(ErrorMessage = "Nick jest wymagany")]
    public string? Username { get; set; }

    [EmailAddress] // To check
    [Required(ErrorMessage = "Email jest wymagany")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    public string Password { get; set; } = null!;
}