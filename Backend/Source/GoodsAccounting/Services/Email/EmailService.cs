using System.Net.Mail;
using System.Net;

namespace GoodsAccounting.Services.Email;

/// <summary>
/// Email service.
/// </summary>
public class EmailService : IEmailService
{
    /// <summary>
    /// Service account smtp client.
    /// </summary>
    private readonly SmtpClient _client;

    /// <summary>
    /// Service account email.
    /// </summary>
    private readonly string _sender;

    /// <summary>
    /// Creating new instance of <see cref="EmailService"/>.
    /// </summary>
    /// <param name="server">SMTP server url.</param>
    /// <param name="port">SMTP server port.</param>
    /// <param name="sender">Service account email.</param>
    /// <param name="password">Service account password.</param>
    public EmailService(string server, int port, string sender, string password)
    {
        _sender = sender;
        _client = new SmtpClient(server, port)
        {
            Credentials = new NetworkCredential(sender, password),
            EnableSsl = true
        };
    }

    /// <inheritdoc />
    public  async Task SendPassword(string email, string password)
    {
        var sender = new MailAddress(_sender, "GoodsAccount service account. No-reply.");
        var recipient = new MailAddress(email);
        var message = new MailMessage(sender, recipient)
        {
            Subject = "Пароль к вашему аккаунту сервиса учета продукции.",
            Body = CreateBody(password)
        };
        
        await _client.SendMailAsync(message);
    }

    /// <summary>
    /// Build common email body message.
    /// </summary>
    /// <param name="password">User's password.</param>
    /// <returns>Target message.</returns>
    private string CreateBody(string password)
    {
        return $"Здравствуйте, для вас был создан новый временный пароль: {password}\nДанное письмо отправлено автоматически. Не стоит отвечать на него.";
    }
}