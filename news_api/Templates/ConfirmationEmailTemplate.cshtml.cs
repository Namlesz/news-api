using Microsoft.AspNetCore.Mvc.RazorPages;

namespace news_api.Templates;

public class ConfirmationEmailTemplate 
{
    public object ConfirmationLink { get; set; } = null!;
}