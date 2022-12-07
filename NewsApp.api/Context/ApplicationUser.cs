using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace NewsApp.api.Context;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? EditorialOfficeId { get; set; }
}