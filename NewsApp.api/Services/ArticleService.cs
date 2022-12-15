using System.Drawing;
using System.Text;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Interfaces.Repositories;
using NewsApp.api.Models;

namespace NewsApp.api.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserService _userService;
    private readonly List<string> _imageTypes = new() { ".jpeg", ".png", ".jpg" };
    private readonly List<string> _contentType = new() { ".html" };

    public ArticleService(IArticleRepository articleRepository, IUserService userService)
    {
        _articleRepository = articleRepository;
        _userService = userService;
    }

    public bool IsAcceptedContentType(IFormFile file)
        => CheckExtension(file, _contentType);

    public bool IsAcceptedImageType(IFormFile file)
        => CheckExtension(file, _imageTypes);

    public async Task<BaseTypeResult<Article>> AddArticle(NewArticle data)
    {
        if (!Guid.TryParse(data.AuthorId, out _))
        {
            return new BaseTypeResult<Article> { Success = false, Message = "Invalid article id" };
        }

        var author = await _userService.FindUser(data.AuthorId);
        if (author is null)
        {
            return new BaseTypeResult<Article> { Success = false, Message = "Author not found" };
        }

        if (string.IsNullOrWhiteSpace(author.EditorialOfficeId))
        {
            return new BaseTypeResult<Article>
                { Success = false, Message = "Author is not assigned to any editorial office" };
        }

        // Read image
        var readImage = await ReadFile(data.Image);

        var article = new ArticleDto()
        {
            Title = data.Title,
            AuthorId = data.AuthorId,
            OfficeId = author.EditorialOfficeId,
            Image = readImage,
            PublishedAt = DateTime.Now
        };

        try
        {
            _articleRepository.Create(article);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseTypeResult<Article> { Success = false, Message = "Something went wrong" };
        }

        return new BaseTypeResult<Article> { Success = true, Data = article };
    }

    public async Task<BaseResult> UpdateContent(string id, string content)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BaseResult { Success = false, Message = "Invalid article id" };
        }

        var article = await _articleRepository.GetArticle(guid);
        if (article is null)
        {
            return new BaseResult { Success = false, Message = "Article not found" };
        }

        article.Content = content;
        var result = await _articleRepository.Update(article);
        if (result.ModifiedCount <= 0)
        {
            return new BaseResult { Success = false, Message = "Something went wrong" };
        }

        return new BaseResult { Success = true };
    }

    //TODO: Add filters logic, return image (check if byte array works) 
    public async Task<List<Article>> GetArticles(string officeId, int range, int offset)
    {
        var articles = await _articleRepository.GetArticles(officeId, range, offset);
        if (articles.Count <= 0)
        {
            return new List<Article>();
        }

        var result = articles.Select(x => new Article()
        {
            Title = x.Title,
            PublishedAt = x.PublishedAt,
            Id = x.Id
        }).ToList();

        return result;
    }

    public async Task<ArticleWithContent?> GetArticle(string articleId)
    {
        if (!Guid.TryParse(articleId, out var guid))
        {
            return null;
        }

        var article = await _articleRepository.GetArticle(guid);
        if (article is null)
        {
            return null;
        }

        return article;
    }

    public async Task<string?> GetArticleThumbnail(string articleId)
    {
        if (!Guid.TryParse(articleId, out var guid))
        {
            return null;
        }

        var thumbnail = await _articleRepository.GetThumbnail(guid);
        if (thumbnail is null)
        {
            return null;
        }

        return thumbnail.Image;
    }

    public async Task<BaseResult> DeleteArticle(string articleId)
    {
        if (!Guid.TryParse(articleId, out var guid))
        {
            return new() { Success = false, Message = "Wrong article id format" };
        }

        try
        {
            var result = await _articleRepository.Delete(guid);
            return new() { Success = result.DeletedCount > 0 };
        }
        catch (Exception e)
        {
            return new() { Success = false, Message = e.Message };
        }
    }

    public async Task<int> GetArticleCounts(string officeId)
    {
        if (!Guid.TryParse(officeId, out var guid))
        {
            return 0;
        }
        
        return await _articleRepository.GetArticleCounts(guid);
    }

#pragma warning disable CA1416
    private async Task<string> ReadFile(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        using var img = Image.FromStream(memoryStream);
        using var m = new MemoryStream();
        img.Save(m, img.RawFormat);
        var imageBytes = m.ToArray();
        string base64String = Convert.ToBase64String(imageBytes);
        return base64String;
    }
#pragma warning restore CA1416

    private bool CheckExtension(IFormFile file, List<string> extensions)
    {
        var extension = Path.GetExtension(file.FileName);
        return extensions.Any(type => type == extension);
    }
}