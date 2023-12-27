using CourierAPI.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace CourierAPI.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendRegisterConfirmationLink(string email, string token, string id)
    {
        var body = new TextPart("html")
        {
            Text = "<h3>Aby zweryfikować adres email kliknij w poniższy link</h3>" +
                "<a href='" + _configuration["CourierWEB:RegisterConfirmURL"] + "?token=" + token + "|" + id + "'>Zweryfikuj</a><br />"
        };

        await SendEmail(email, "Weryfikacja konta", body);
    }

    public async Task SendResetPasswordLink(string email, string token, string id)
    {
        var body = new TextPart("html")
        {
            Text = "<h3>Aby zresetować hasło kliknij w poniższy link</h3>" +
                "<a href='" + _configuration["CourierWEB:ResetPasswordURL"] + "?token=" + token + "|" + id + "'>Reset hasła</a><br />"
        };

        await SendEmail(email, "Reset hasła", body);
    }

    private async Task SendEmail(string email, string subject, MimeEntity body)
    {
        string mailFrom = _configuration["MailService:Email"] ?? throw new InvalidOperationException("Email not found in configuration.");
        MimeMessage message = new();
        message.From.Add(new MailboxAddress("Kurier UWM", mailFrom));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;

        message.Body = body;

        string key = _configuration["MailService:Key"] ?? throw new InvalidOperationException("Key not found in configuration.");

        using (MailKit.Net.Smtp.SmtpClient client = new())
        {
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate(mailFrom, key);
            await client.SendAsync(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
