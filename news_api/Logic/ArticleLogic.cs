using System.Text;
using news_api.Interfaces.Logic;
using news_api.Interfaces.Repositories;
using news_api.Models;

namespace news_api.Logic;

public class ArticleLogic : IArticleLogic
{
    private readonly IArticleRepository _articleRepository;
    private readonly IApplicationUserLogic _applicationUserLogic;
    private readonly List<string> _imageTypes = new() { ".jpeg", ".png" };
    private readonly List<string> _contentType = new() { ".html" };

    public ArticleLogic(IArticleRepository articleRepository, IApplicationUserLogic applicationUserLogic)
    {
        _articleRepository = articleRepository;
        _applicationUserLogic = applicationUserLogic;
    }

    public bool IsAcceptedContentType(IFormFile file)
        => CheckExtension(file, _contentType);
    
    public bool IsAcceptedImageType(IFormFile file)
        => CheckExtension(file, _imageTypes);
    
    public async Task<BaseResult> AddArticle(NewArticle data)
    {
        var author = await _applicationUserLogic.FindUser(data.AuthorId);
        if (author is null)
        {
            return new BaseResult { Success = false, Message = "Author not found" };
        }

        if (string.IsNullOrWhiteSpace(author.EditorialOfficeId))
        {
            return new BaseResult { Success = false, Message = "Author is not assigned to any editorial office" };
        }

        // Read file content
        var readContent = await ReadFile(data.Content);
        byte[] contentDataBytes = Encoding.UTF8.GetBytes(readContent);
        
        // Read image
        var readImage = await ReadFile(data.Image);
        byte[] imageDataBytes = Encoding.UTF8.GetBytes(readImage);

        var article = new ArticleWithContent()
        {
            Title = data.Title,
            Content = contentDataBytes,
            AuthorId = data.AuthorId,
            OfficeId = author.EditorialOfficeId,
            Image = imageDataBytes,
            PublishedAt = DateTime.Now
        };

        try
        {
            _articleRepository.Create(article);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResult { Success = false, Message = "Something went wrong" };
        }

        return new BaseResult { Success = true, Message = "Article added" };
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
            Image = x.Image,
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

        return await _articleRepository.GetArticle(guid);
    }

    private async Task<string> ReadFile(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
    
    private bool CheckExtension(IFormFile file, List<string> extensions)
    {
        var extension = Path.GetExtension(file.FileName);
        return extensions.Any(type => type == extension);
    }
}