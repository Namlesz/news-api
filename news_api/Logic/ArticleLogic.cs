using System.Text;
using news_api.Interfaces.Logic;
using news_api.Interfaces.Repositories;
using news_api.Models;

namespace news_api.Logic;

public class ArticleLogic : IArticleLogic
{
    private readonly IArticleRepository _articleRepository;
    private readonly IApplicationUserLogic _applicationUserLogic;

    public ArticleLogic(IArticleRepository articleRepository, IApplicationUserLogic applicationUserLogic)
    {
        _articleRepository = articleRepository;
        _applicationUserLogic = applicationUserLogic;
    }

    public bool IsAcceptedType(IFormFile file) =>
        Equals(Path.GetExtension(file.FileName), ".html");

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

        var readContent = await ReadFile(data.Content);
        byte[] fileData = Encoding.UTF8.GetBytes(readContent);

        var article = new ArticleWithContent()
        {
            Title = data.Title,
            Description = data.Description,
            Content = fileData,
            AuthorId = data.AuthorId,
            OfficeId = author.EditorialOfficeId,
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
            Description = x.Description,
            PublishedAt = x.PublishedAt,
            Id = x.Id
        }).ToList();

        return result;
    }


    private async Task<string> ReadFile(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}