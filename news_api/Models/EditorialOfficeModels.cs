using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace news_api.Models;

[CollectionName("EditorialOffices")]

public class EditorialOfficeInfo
{
    public string? Name { get; set; }
    public string? Town { get; set; }
    public string? OwnerInfo { get; set; }
}

public class EditorialOffice : EditorialOfficeInfo
{
    [BsonId] public Guid Id { get; set; }
    public string? OwnerId { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Town) && !string.IsNullOrEmpty(OwnerId);
    }
    
    public EditorialOfficeInfo ToInfo()
    {
        return new()
        {
            Name = Name,
            Town = Town,
            OwnerInfo = OwnerInfo
        };
    }
}