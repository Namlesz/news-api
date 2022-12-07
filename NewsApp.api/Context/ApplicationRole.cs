using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace NewsApp.api.Context;

[CollectionName("Roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{
    
}