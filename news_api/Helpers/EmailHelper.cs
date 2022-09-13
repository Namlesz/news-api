using System.Net.Mail;

namespace news_api.Helpers;

public class EmailHelper
{
    private readonly string _password;
    private readonly string _email;

    public EmailHelper()
    {
         _password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new InvalidOperationException();
         _email = Environment.GetEnvironmentVariable("EMAIL_ACCOUNT") ?? throw new InvalidOperationException();
    }
        
    public bool SendConfirmEmail(string userEmail, string confirmationLink)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_email);
        mailMessage.To.Add(new MailAddress(userEmail));
        mailMessage.Subject = "Confirm your email";
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = confirmationLink;
 
        SmtpClient client = new SmtpClient();
        client.Credentials = new System.Net.NetworkCredential(_email, _password);
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.EnableSsl = true;
 
        try
        {
            client.Send(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
                Console.WriteLine(ex);
        }
        return false;
    }
    
    public bool SendResetPasswordEmail(string userEmail, string passwordResetLink)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_email);
        mailMessage.To.Add(new MailAddress(userEmail));
 
        mailMessage.Subject = "Change your password";
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = passwordResetLink;
 
        SmtpClient client = new SmtpClient();
        client.Credentials = new System.Net.NetworkCredential(_email, _password);
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.EnableSsl = true;
 
        try
        {
            client.Send(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
                Console.WriteLine(ex);
        }
        return false;
    }
}