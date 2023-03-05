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
    /// <param name="soldGoods">Sold goods dictionary.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shift.</exception>
    Task UpdateSoldGoodsAsync(Dictionary<Guid, int> soldGoods);

    /// <summary>
    /// Initialize new working shift.
    /// </summary>
    /// <param name="id">User's identifier, who opened shift.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityExistsException"><see cref="EntityExistsException"/> for working shift.</exception>
    Task InitWorkShiftAsync(int id);

    /// <summary>
    /// Closing today's working shift.
    /// </summary>
    /// <param name="id">User's identifier, who opened shift.</param>
    /// <param name="cash">Cash in cash machine.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"><see cref="EntityNotFoundException"/> for working shift.</exception>
    Task CloseWorkShiftAsync(int id, int cash);

    /// <summary>
    /// Getting history aggregated information.
    /// </summary>
    /// <param name="id">Identifier of user who requested aggregated data.</param>
    /// <param name="date">Date for snapshot.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<IList<WorkShift>> GetWorkShiftSnapshotsAsync(int id, DateOnly date);
}