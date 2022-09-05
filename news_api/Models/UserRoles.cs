using MongoDbGenericRepository.Attributes;

namespace news_api.Auth;

[CollectionName("Roles")]
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";
}