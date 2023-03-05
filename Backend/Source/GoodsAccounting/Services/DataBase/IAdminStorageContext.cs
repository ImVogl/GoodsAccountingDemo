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
    /// <param name="remove">Goods to remove.</param>
    /// <param name="goods">List of <see cref="GoodsItem"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for user.</exception>
    Task UpdateGoodsStorageAsync(int userId, List<Guid> remove, IList<GoodsItem> goods);
}