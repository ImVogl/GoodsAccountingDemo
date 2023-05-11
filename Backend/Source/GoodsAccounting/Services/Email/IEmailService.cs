namespace GoodsAccounting.Services.Email;

/// <summary>
/// Interface of email service.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sending password via email service.
    /// </summary>
    /// <param name="email">Target email.</param>
    /// <param name="password">User's password.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SendPassword(string email, string password);
}