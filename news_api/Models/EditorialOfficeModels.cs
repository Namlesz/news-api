using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace news_api.Models;

[CollectionName("EditorialOffices")]
public class EditorialOffice
{
    [BsonId] public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Town { get; set; }
    public string? OwnerInfo { get; set; }
    public string? OwnerId { get; set; }
    //public Image? Logo { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Town) && !string.IsNullOrEmpty(OwnerId);
    }
}
