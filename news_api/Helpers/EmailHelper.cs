using System.Net.Mail;

namespace news_api.Helpers;

public class EmailHelper
{
    public bool SendConfirmEmail(string userEmail, string confirmationLink)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("nejmlesz@gmail.com");
        mailMessage.To.Add(new MailAddress(userEmail));
 
        mailMessage.Subject = "Confirm your email";
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = confirmationLink;
 
        SmtpClient client = new SmtpClient();
        var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        var email = Environment.GetEnvironmentVariable("EMAIL_ACCOUNT");
        client.Credentials = new System.Net.NetworkCredential(email, password);
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