using GoodsAccounting.Model.DTO;
using GoodsAccounting.Services.SnapshotConverter;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class DisplayNamesUpdaterTests
{
    private static readonly DisplayNamesUpdater Updater = new ();

    [Test]
    [Description("Update displayed names for full dto list.")]
    public void UpdateNamesTest()
    {
        var list = new List<ShiftSnapshotDto>
        {
            new() { Cash = 0, UserDisplayName = "Name", StorageItems = new List<StorageItemInfoDto>() },
            new() { Cash = 0, UserDisplayName = "Name", StorageItems = new List<StorageItemInfoDto>() },
            new() { Cash = 0, UserDisplayName = "AnotherName", StorageItems = new List<StorageItemInfoDto>() },
            new() { Cash = 0, UserDisplayName = "Name", StorageItems = new List<StorageItemInfoDto>() }
        };

        var expectedCount = list.Count;
        Updater.UpdateSnapshots(list);

        Assert.That(list.Select(dto => dto.UserDisplayName).Distinct().Count(), Is.EqualTo(expectedCount));
        Assert.That(list.Any(dto => dto.UserDisplayName == "Name"), Is.True);
        Assert.That(list.Any(dto => dto.UserDisplayName == "Name 1"), Is.True);
        Assert.That(list.Any(dto => dto.UserDisplayName == "Name 2"), Is.True);
        Assert.That(list.Any(dto => dto.UserDisplayName == "AnotherName"), Is.True);
    }

    [Test]
    [Description("Update displayed names for reduced dto list.")]
    public void UpdateNamesReducedTest()
    {
        var list = new List<ReducedSnapshotDto>
        {
            new() { Cash = 0, UserDisplayName = "Name", StorageItems = new List<ReducedItemInfoDto>() },
            new() { Cash = 0, UserDisplayName = "Name", StorageItems = new List<ReducedItemInfoDto>() },
            new() { Cash = 0, UserDisplayName = "AnotherName", StorageItems = new List<ReducedItemInfoDto>() },
            new() { Cash = 0, UserDisplayName = "Name", StorageItems = new List<ReducedItemInfoDto>() }
        };

        var expectedCount = list.Count;
        Updater.UpdateSnapshots(list);

        Assert.That(list.Select(dto => dto.UserDisplayName).Distinct().Count(), Is.EqualTo(expectedCount));
        Assert.That(list.Any(dto => dto.UserDisplayName == "Name"), Is.True);
        Assert.That(list.Any(dto => dto.UserDisplayName == "Name 1"), Is.True);
        Assert.That(list.Any(dto => dto.UserDisplayName == "Name 2"), Is.True);
        Assert.That(list.Any(dto => dto.UserDisplayName == "AnotherName"), Is.True);
    }
}
