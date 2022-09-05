using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace news_api.Settings;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
}