using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace NewsApp.api.Models;

public class Article
{
    [BsonId] public Guid Id { get; init; }
    public string? Title { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class ArticleImage : Article
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte[]? Image { get; set; }
}

[BsonIgnoreExtraElements]
[CollectionName("Articles")]
public class ArticleDto : ArticleImage
{
    public string? AuthorId { get; set; }
    public string? OfficeId { get; init; }
}

[BsonIgnoreExtraElements]
[CollectionName("Articles")]
public class ArticleWithContent : ArticleDto
{
    public string? Content { get; set; }
}

[BsonIgnoreExtraElements]
[CollectionName("Articles")]
public record ArticleThumbnail(Guid Id, byte[] Image);

public record NewArticle(
    [Required] string Title,
    [Required] string AuthorId,
    [Required] IFormFile Image
);

public record ArticleContent(
    [Required] string Id,
    [Required] string Content
);