using GoodsAccounting.Model.DTO;

namespace GoodsAccounting.Services.SnapshotConverter;

/// <summary>
/// Service snapshot names updater.
/// </summary>
public class DisplayNamesUpdater
{
    /// <summary>
    /// Update snapshots.
    /// </summary>
    /// <param name="list">List of <see cref="ShiftSnapshotDto"/>.</param>
    public void UpdateSnapshots(List<ShiftSnapshotDto> list)
    {
        for (var i = 0; i < list.Count; ++i)
        {
            var count = 0;
            var baseName = list[i].UserDisplayName;
            while (list.Count(dto => dto.UserDisplayName == list[i].UserDisplayName) > 1)
                list[i].UserDisplayName = $"{baseName} {++count}";
        }
    }

    /// <summary>
    /// Update snapshots.
    /// </summary>
    /// <param name="list">List of <see cref="ReducedSnapshotDto"/>.</param>
    public void UpdateSnapshots(List<ReducedSnapshotDto> list)
    {
        for (var i = 0; i < list.Count; ++i)
        {
            var count = 0;
            var baseName = list[i].UserDisplayName;
            while (list.Count(dto => dto.UserDisplayName == list[i].UserDisplayName) > 1)
                list[i].UserDisplayName = $"{baseName} {++count}";
        }
    }
}