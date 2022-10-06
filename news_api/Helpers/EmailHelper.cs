using System.Net.Mail;
using news_api.Templates;
using Razor.Templating.Core;

namespace news_api.Helpers;

public class EmailHelper
{
    private readonly MailMessage _mailMessage;
    private readonly SmtpClient _smtpClient;

    public EmailHelper()
    {
        var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new InvalidOperationException();
        var email = Environment.GetEnvironmentVariable("EMAIL_ACCOUNT") ?? throw new InvalidOperationException();

        _mailMessage = new MailMessage();
        _mailMessage.From = new MailAddress(email);
        _mailMessage.IsBodyHtml = true;

        _smtpClient = new SmtpClient("smtp.gmail.com", 587);
        _smtpClient.Credentials = new System.Net.NetworkCredential(email, password);
        _smtpClient.EnableSsl = true;
    }

    public bool SendConfirmEmail(string userEmail, string confirmationLink)
    {
        _mailMessage.To.Add(new MailAddress(userEmail));
        _mailMessage.Subject = "Confirm your email";
        _mailMessage.IsBodyHtml = true;

        var htmlBody = RazorTemplateEngine.RenderAsync("~/Templates/ConfirmationEmailTemplate.cshtml",
            new ConfirmationEmailTemplate { ConfirmationLink = confirmationLink }).Result;
        _mailMessage.Body = htmlBody;

        try
        {
            _smtpClient.Send(_mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
                Console.WriteLine(ex);
        }
        finally
        {
            _mailMessage.Dispose();
            _smtpClient.Dispose();
        }

        return false;
    }

    public bool SendResetPasswordEmail(string userEmail, string passwordResetLink)
    {
        _mailMessage.To.Add(new MailAddress(userEmail));
        _mailMessage.Subject = "Change your password";
        _mailMessage.IsBodyHtml = true;

        var htmlBody = RazorTemplateEngine.RenderAsync("~/Templates/ResetPasswordTemplate.cshtml",
            new ResetPasswordTemplate() { ResetLink = passwordResetLink }).Result;
        _mailMessage.Body = htmlBody;
        
        try
        {
            _smtpClient.Send(_mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
                Console.WriteLine(ex);
        }
        finally
        {
            _mailMessage.Dispose();
            _smtpClient.Dispose();
        }

        return false;
    }
}