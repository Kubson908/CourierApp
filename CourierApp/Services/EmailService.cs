using CourierAPI.Helpers;
using CourierAPI.Models.Dto;
using MimeKit;

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

    public async Task SendLabels(string email, List<LabelShipmentDto> shipments)
    {
        FileInfoDto file = PDFLabelHelper.GenerateLabels(shipments);
        string bodyFragment = shipments.Count > 1 ? "<p>W załączniku dołączono etykiety przesyłek</p>" : "<p>W załączniku dołączono etykietę przeyłki</p>";
        var builder = new BodyBuilder();
        builder.HtmlBody = "<h3>Dziękujemy za zarejestrowanie przesyłek</h3>" +
                "<p>Szczegóły zamówienia możesz zobaczyć w historii zamówień na swoim profilu użytkownika</p>" + bodyFragment;
        builder.Attachments.Add(file.Name, file.Bytes, new ContentType("application", file.Type.Split("/")[1]));
        await SendEmail(email, "Potwierdzenie zamówienia", builder.ToMessageBody());
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
