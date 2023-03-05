using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Goods storage entity framework context interface.
/// </summary>
public interface IStorageContext : IDisposable
{
    /// <summary>
    /// Get database set for <see cref="GoodsItem"/>.
    /// </summary>
    DbSet<GoodsItem> Goods { get; }

    /// <summary>
    /// Get database set for <see cref="WorkShift"/>.
    /// </summary>
    DbSet<WorkShift> WorkShifts { get; }
    
    /// <summary>
    /// Update sold goods.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="soldGoods">Sold goods dictionary.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shift.</exception>
    Task UpdateSoldGoodsAsync(int userId, Dictionary<Guid, int> soldGoods);

    /// <summary>
    /// Initialize new working shift and add it to table.
    /// </summary>
    /// <param name="id">User's identifier, who opened shift.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityExistsException"><see cref="EntityExistsException"/> for working shift.</exception>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for user entity.</exception>
    Task InitWorkShiftAsync(int id);

    /// <summary>
    /// Closing today's working shift.
    /// </summary>
    /// <param name="id">User's identifier, who opened shift.</param>
    /// <param name="cash">Cash in cash machine.</param>
    /// <param name="comment">Comment.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shift.</exception>
    /// <exception cref="InvalidOperationException">This exception throws if table contains two opened shifts.</exception>
    Task CloseWorkShiftAsync(int id, int cash, string comment = "");

    /// <summary>
    /// Getting history aggregated information.
    /// </summary>
    /// <param name="id">Identifier of user who requested aggregated data.</param>
    /// <param name="date">Date for snapshot.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<IList<WorkShift>> GetWorkShiftSnapshotsAsync(int id, DateOnly date);
}