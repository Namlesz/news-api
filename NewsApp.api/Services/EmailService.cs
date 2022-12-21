using System.Net.Mail;
using NewsApp.api.Templates;
using Razor.Templating.Core;

namespace NewsApp.api.Services;

public class EmailService
{
    private readonly MailMessage _mailMessage;
    private readonly SmtpClient _smtpClient;

    public EmailService()
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

    /// <summary>
    /// Sends a confirmation email to the user.
    /// </summary>
    /// <param name="userEmail">Target email</param>
    /// <param name="confirmationLink">Generated confirm URL</param>
    /// <returns>True if success else false.</returns>
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
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
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

    /// <summary>
    /// Sends a reset password email to the user.
    /// </summary>
    /// <param name="userEmail">Target email</param>
    /// <param name="passwordResetLink">Generated reset URL</param>
    /// <returns></returns>
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