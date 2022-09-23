using MongoDbGenericRepository.Attributes;

namespace news_api.Settings;

[CollectionName("Roles")]
public static class UserRoles
{
    public const string Admin = nameof(Roles.Admin);
    public const string Editor = nameof(Roles.Editor);
}

public enum Roles
{
    Admin,
    Editor
}