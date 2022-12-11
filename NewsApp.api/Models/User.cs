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
    [Required]
    string Name,
    [Required]
    string Surname,
    [EmailAddress]
    [Required]
    string Email,
    [Required]
    string Password
);