using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace news_api.Settings;

[CollectionName("Roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{
    
}