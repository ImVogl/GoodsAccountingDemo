using GoodsAccounting.Model.Exceptions;

namespace GoodsAccounting.Services.Password;

using JetBrains.Annotations;

/// <summary>
/// Interface for password service.
/// </summary>
public interface IPassword
{
    /// <summary>
    /// Calculate password hash.
    /// </summary>
    /// <param name="password">Hashing password.</param>
    /// <param name="salt">Salt.</param>
    /// <returns>Tuple with salt and hashed password.</returns>
    /// <exception cref="BadPasswordException"><see cref="BadPasswordException"/></exception>
    [NotNull]
    [ItemNotNull]
    (string, byte[]) Hash([NotNull] string password, [CanBeNull] string? salt = null);

    /// <summary>
    /// Verification passwords
    /// </summary>
    /// <param name="password">Verifiable password.</param>
    /// <param name="hashedPassword">Hashed password for compassion.</param>
    /// <param name="salt">Salt.</param>
    /// <returns>Value is indicating that password was verified.</returns>
    /// <exception cref="BadPasswordException"><see cref="BadPasswordException"/></exception>
    /// <exception cref="ArgumentNullException">Salt or hashed password is null or empty.</exception>
    bool VerifyPassword([NotNull] byte[] hashedPassword, [NotNull] string password, [NotNull] string salt);

    /// <summary>
    /// Generating random password.
    /// </summary>
    /// <returns>Password.</returns>
    [NotNull]
    string GeneratePassword();
}