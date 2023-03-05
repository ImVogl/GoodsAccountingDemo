using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Interface for users access data base proxy.
/// </summary>
public interface IUsersContext : IDisposable
{
    /// <summary>
    /// Get collection of <see cref="User"/> entity.
    /// </summary>
    [NotNull]
    [ItemNotNull]
    DbSet<User> Users { get; }

    /// <summary>
    /// Change password for target user.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="salt">Password salt.</param>
    /// <param name="hash">Hashed password.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException">User's <see cref="EntityNotFoundException"/></exception>
    Task ChangePasswordAsync(int id, string salt, byte[] hash);

    /// <summary>
    /// Adding new user.
    /// </summary>
    /// <param name="user"><see cref="User"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityExistsException">User's <see cref="EntityExistsException"/>.</exception>
    Task AddUserAsync([NotNull] User user);

    /// <summary>
    /// Remove user with target identifier.
    /// </summary>
    /// <param name="id">User's identifier.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveUserAsync(int id);

    /// <summary>
    /// Check user in "users" table.
    /// </summary>
    /// <param name="login">User's login.</param>
    /// <param name="name">User's name.</param>
    /// <param name="surname">User's surname.</param>
    /// <param name="birthDay">User's birth day.</param>
    /// <returns><see cref="Task"/> with value than means: the user exists in the table.</returns>
    Task<bool> DoesUserExistsAsync(string login, string name, string surname, DateOnly birthDay);
}