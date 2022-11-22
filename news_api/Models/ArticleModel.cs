using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace news_api.Models;

public class Article
{
    [BsonId] public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Byte[]? Image { get; set; }
}

[BsonIgnoreExtraElements]
[CollectionName("Articles")]
public class ArticleDto : Article
{
    public string? AuthorId { get; set; }
    public string? OfficeId { get; set; }
}

[BsonIgnoreExtraElements]
[CollectionName("Articles")]
public class ArticleWithContent : ArticleDto
{
    public Byte[]? Content { get; set; }
}

public record NewArticle(
    [Required(ErrorMessage = "Title is required")]
    string Title,
    [Required(ErrorMessage = "AuthorId is required")]
    string AuthorId,
    [Required(ErrorMessage = "Content is required")]
    IFormFile Content,
    [Required(ErrorMessage = "Image is required")]
    IFormFile Image
);