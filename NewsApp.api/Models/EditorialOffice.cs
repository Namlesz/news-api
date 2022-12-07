using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace NewsApp.api.Models;

public class EditorialOffice
{
    public string? Name { get; set; }
    public string? Town { get; set; }
}

public class OfficeInfo : EditorialOffice
{
    public string? OwnerInfo { get; set; }
}

[CollectionName("EditorialOffices")]
public class EditorialOfficeDto : EditorialOffice
{
    [BsonId] public Guid Id { get; set; }
    public string? OwnerId { get; set; }
}

public record NewOffice(
    [Required(ErrorMessage = "Name is required")]
    string Name,
    [Required(ErrorMessage = "Town is required")]
    string Town,
    [Required(ErrorMessage = "Owner id is required")]
    string OwnerId);