using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.DTO;

namespace GoodsAccounting.Services.SnapshotConverter;

/// <summary>
/// Interface for data base entity to snapshots collection converter.
/// </summary>
public interface ISnapshotConverter
{
    /// <summary>
    /// Converting list of <see cref="WorkShift"/> to list of <see cref="ShiftSnapshotDto"/>.
    /// </summary>
    /// <param name="shifts">List of <see cref="WorkShift"/>.</param>
    /// <param name="goods">List of <see cref="GoodsItem"/>.</param>
    /// <returns>List of <see cref="ShiftSnapshotDto"/>.</returns>
    public IList<ShiftSnapshotDto> Convert(IList<WorkShift> shifts, IList<GoodsItem> goods);

    /// <summary>
    /// Converting list of <see cref="WorkShift"/> to list of <see cref="ReducedSnapshotDto"/>.
    /// </summary>
    /// <param name="shifts">List of <see cref="WorkShift"/>.</param>
    /// <param name="goods">List of <see cref="GoodsItem"/>.</param>
    /// <returns>List of <see cref="ReducedSnapshotDto"/>.</returns>
    public IList<ReducedSnapshotDto> ConvertReduced(IList<WorkShift> shifts, IList<GoodsItem> goods);
}