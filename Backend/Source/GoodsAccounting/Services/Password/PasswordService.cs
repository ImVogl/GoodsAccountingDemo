using System.Security.Cryptography;
using GoodsAccounting.Model.Exceptions;
using System.Text;
using GoodsAccounting.Services.Validator;
using ILogger = NLog.ILogger;

namespace GoodsAccounting.Services.Password;

/// <summary>
/// Password service.
/// </summary>
public class PasswordService : IPassword
{
    /// <summary>
    /// Logger.
    /// </summary>
    private static readonly ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Instance of <see cref="IPasswordValidator"/>.
    /// </summary>
    private readonly IPasswordValidator _validator;

    /// <summary>
    /// Create instance of <see cref="PasswordService"/>.
    /// </summary>
    /// <param name="validator">Instance of <see cref="IPasswordValidator"/>.</param>
    public PasswordService(IPasswordValidator validator)
    {
        _validator = validator;
    }

    /// <inheritdoc />
    public (string, byte[]) Hash(string password, string? salt)
    {
        if (!_validator.Validate(password))
        {
            Logger.Error("Invalid password.");
            throw new BadPasswordException();
        }

        var internalSalt = salt ?? Guid.NewGuid().ToString();
        var saltBytes = Encoding.UTF8.GetBytes(internalSalt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var plainTextWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];
        for (var i = 0; i < passwordBytes.Length; i++)
            plainTextWithSaltBytes[i] = passwordBytes[i];

        for (var i = 0; i < saltBytes.Length; i++)
            plainTextWithSaltBytes[passwordBytes.Length + i] = saltBytes[i];

        var algorithm = SHA256.Create();
        return (internalSalt, algorithm.ComputeHash(plainTextWithSaltBytes));
    }

    /// <inheritdoc />
    public bool VerifyPassword(byte[] hash, string password, string salt)
    {
        if (!_validator.Validate(password))
        {
            Logger.Error("Invalid password.");
            throw new BadPasswordException();
        }

        if (hash == null || hash.Length == 0)
        {
            Logger.Error("Hash is null or empty.");
            throw new ArgumentNullException(nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(salt))
        {
            Logger.Error("Salt is null or empty.");
            throw new ArgumentNullException(nameof(salt));
        }

        var (_, hashedPassword) = Hash(password, salt);
        if (hashedPassword.Length != hash.Length)
            return false;

        return !hashedPassword.Where((t, i) => t != hash[i]).Any();
    }

    /// <inheritdoc />
    public string GeneratePassword()
    {
        const byte firstAnsi = 32;
        const byte lastAnsi = 126;
        const int passwordLength = 12;
        var password = new char[passwordLength];
        var random = new Random();
        for (var i = 0; i < passwordLength; ++i)
            password[i] = (char)random.Next(firstAnsi, lastAnsi);

        return (new string(password)) + "1qQ*";
    }
}