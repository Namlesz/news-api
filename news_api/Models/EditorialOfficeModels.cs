using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace news_api.Models;

public class EditorialOffice
{
    public string? Name { get; set; }
    public string? Town { get; set; }
    public string? OwnerInfo { get; set; }

    public EditorialOffice ToBase() => new()
    {
        Name = Name,
        Town = Town,
        OwnerInfo = OwnerInfo
    };
}

[CollectionName("EditorialOffices")]
public class EditorialOfficeDto : EditorialOffice
{
    [BsonId] public Guid Id { get; set; }
    public string? OwnerId { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Town) && !string.IsNullOrEmpty(OwnerId);
    }
}