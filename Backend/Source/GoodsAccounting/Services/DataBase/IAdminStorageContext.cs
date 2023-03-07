using GoodsAccounting.Model;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;

namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Goods storage entity framework context interface.
/// </summary>
public interface IAdminStorageContext : IStorageContext
{
    /// <summary>
    /// Updating goods storage.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="changing">List of <see cref="GoodsItemStateChanging"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shifts DbSet entity.</exception>
    /// <exception cref="TableAccessException"><see cref="TableAccessException"/>.</exception>
    Task UpdateGoodsStorageAsync(int userId, Dictionary<Guid, GoodsItemStateChanging> changing);
    
    /// <summary>
    /// Renaming goods item in storage.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="id">Item identifier.</param>
    /// <param name="newName">New item name.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shifts DbSet entity.</exception>
    /// <exception cref="TableAccessException"><see cref="TableAccessException"/>.</exception>
    Task RenameGoodsItemAsync(int userId, Guid id, string newName);

    /// <summary>
    /// Adding new item into storage.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="newItem"><see cref="GoodsItem"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shifts DbSet entity.</exception>
    /// <exception cref="EntityExistsException"><see cref="EntityExistsException"/> for goods DbSet entity.</exception>
    /// <exception cref="TableAccessException"><see cref="TableAccessException"/>.</exception>
    Task AddNewGoodsItemAsync(int userId, GoodsItem newItem);

    /// <summary>
    /// Remove item from storage.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="id">Identifier of <see cref="GoodsItem"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shifts DbSet entity.</exception>
    /// <exception cref="TableAccessException"><see cref="TableAccessException"/>.</exception>
    Task RemoveGoodsItemAsync(int userId, Guid id);

    /// <summary>
    /// Restore removed item in storage.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="id">Identifier of <see cref="GoodsItem"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shifts DbSet entity.</exception>
    /// <exception cref="TableAccessException"><see cref="TableAccessException"/>.</exception>
    Task RestoreGoodsItemAsync(int userId, Guid id);
}