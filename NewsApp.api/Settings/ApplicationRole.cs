using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace NewsApp.api.Settings;

[CollectionName("Roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{
    
}