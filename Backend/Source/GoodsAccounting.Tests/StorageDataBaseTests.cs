﻿using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.DataBase;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class StorageDataBaseTests
{
    private static readonly DbContextOptions Options = new DbContextOptionsBuilder<PostgresProxy>()
        .UseInMemoryDatabase(databaseName: "goods_account")
        .EnableSensitiveDataLogging()
        .Options;

    [Test]
    [Description("Try to initialize new working shift when opened shift exists")]
    public async Task ThrowOpenedShiftExistsTest()
    {
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        var closedShift = new WorkShift
        {
            Cash = 100,
            IsOpened = true,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Parse("2000-01-01 06:35"),
            UserId = 1,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>()
        };

        await context.WorkShifts.AddAsync(closedShift);
        await context.SaveChangesAsync();
        Assert.ThrowsAsync<EntityExistsException>(async () => await context.InitWorkShiftAsync(1));
    }

    [Test]
    [Description("Try to initialize new working shift when user with target identifier doesn't exist")]
    public async Task ThrowInitShiftNoUserTest()
    {
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        var closedShift = new WorkShift
        {
            Cash = 100,
            IsOpened = false,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Parse("2000-01-01 06:35"),
            UserId = 1,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>()
        };

        await context.WorkShifts.AddAsync(closedShift);
        await context.SaveChangesAsync();
        Assert.ThrowsAsync<EntityNotFoundException>(async () => await context.InitWorkShiftAsync(1));
    }

    [Test]
    [Description("Create new working shift.")]
    public async Task InitNewShiftTest()
    {
        const int id = 1;
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(new User
        {
            Hash = Array.Empty<byte>(),
            Id = id,
            Name = "Test",
            Salt = "111",
            Surname = "Test2",
            UserLogin = string.Empty,
            Role = string.Empty
        });

        var firstItemId = Guid.NewGuid();
        var secondItemId = Guid.NewGuid();
        var thirdItemId = Guid.NewGuid();
        await context.Goods.AddRangeAsync(
            new GoodsItem
                { Id = firstItemId, Actives = true, Name = "First", RetailPrice = 100F, WholeScalePrice = 80F, CurrentItemsInStorageCount = 50 },
            new GoodsItem
                { Id = secondItemId, Actives = true, Name = "Second", RetailPrice = 150F, WholeScalePrice = 85F, CurrentItemsInStorageCount = 150 },
            new GoodsItem
                { Id = thirdItemId, Actives = false, Name = "Third", RetailPrice = 250F, WholeScalePrice = 185F, CurrentItemsInStorageCount = 250 }
        );
        
        await context.SaveChangesAsync();
#pragma warning disable CS8618

        Assert.That(context.WorkShifts.Any(), Is.False);
        await context.InitWorkShiftAsync(id);
        Assert.That(context.WorkShifts.Count(), Is.EqualTo(1));

        var target = await context.WorkShifts.FirstAsync();
        Assert.That(target.UserId, Is.EqualTo(id));
        Assert.That(target.GoodItemStates.Count, Is.EqualTo(2));
        Assert.That(target.GoodItemStates[0].Id, Is.EqualTo(firstItemId));
        Assert.That(target.GoodItemStates[1].Id, Is.EqualTo(secondItemId));
        
        Assert.That(target.GoodItemStates[0].RetailPrice, Is.EqualTo(100F));
        Assert.That(target.GoodItemStates[1].RetailPrice, Is.EqualTo(150F));

        Assert.That(target.GoodItemStates[0].WholeScalePrice, Is.EqualTo(80F));
        Assert.That(target.GoodItemStates[1].WholeScalePrice, Is.EqualTo(85F));
#pragma warning restore CS8618
    }

    [Test]
    [Description("Try to close not opened shift")]
    public async Task ThrowNoOpenedShiftsTest()
    {
        const int id = 1;
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        var closedShift = new WorkShift
        {
            Cash = 100,
            IsOpened = false,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Parse("2000-01-01 06:35"),
            UserId = id,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>()
        };
        
        await context.WorkShifts.AddAsync(closedShift);
        await context.SaveChangesAsync();
        Assert.ThrowsAsync<EntityNotFoundException>(async () => await context.CloseWorkShiftAsync(id, 0));
    }
    
    [Test]
    [Description("Try to close shift when table has two opened shifts")]
    public async Task ThrowManyShiftsTest()
    {
        const int id = 1;
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        var openedShift = new WorkShift
        {
            Cash = 100,
            IsOpened = true,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Parse("2000-01-01 06:35"),
            UserId = id,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>()
        };

        await AddWorkShiftsAsync(context, DateTime.Today, DateTime.Now);
        await context.WorkShifts.AddAsync(openedShift);
        await context.SaveChangesAsync();
        Assert.ThrowsAsync<InvalidOperationException>(async () => await context.CloseWorkShiftAsync(id, 0));
    }
    
    [Test]
    [Description("Close valid scheme")]
    public async Task CloseValidShiftTest()
    {
        const int id1 = 1;
        const int id2 = 2;
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var firstItemId = Guid.NewGuid();
        var secondItemId = Guid.NewGuid();
        var thirdItemId = Guid.NewGuid();
        await context.Goods.AddRangeAsync(
            new GoodsItem
                { Id = firstItemId, Actives = true, Name = "First", RetailPrice = 100F, WholeScalePrice = 80F, CurrentItemsInStorageCount = 350 },
            new GoodsItem
                { Id = secondItemId, Actives = true, Name = "Second", RetailPrice = 150F, WholeScalePrice = 85F, CurrentItemsInStorageCount = 150 },
            new GoodsItem
                { Id = thirdItemId, Actives = false, Name = "Third", RetailPrice = 250F, WholeScalePrice = 185F, CurrentItemsInStorageCount = 250 }
        );
        
        var openedShiftId1 = new WorkShift
        {
            Cash = 100,
            IsOpened = true,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Now.AddDays(5),
            UserId = id1,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>
            {
                new() { Id = firstItemId, RetailPrice = 100F, WholeScalePrice = 80F, WriteOff = 15, Receipt = 25, Sold = 40 },
                new () { Id = secondItemId, RetailPrice = 150F, WholeScalePrice = 85F, WriteOff = 0, Receipt = 0, Sold = 50 }
            }
        };
        var openedShiftId2 = new WorkShift
        {
            Cash = 100,
            IsOpened = true,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Now.AddDays(5),
            UserId = id2,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>
            {
                new() { Id = firstItemId, RetailPrice = 100F, WholeScalePrice = 80F, WriteOff = 15, Receipt = 25, Sold = 40 },
                new () { Id = secondItemId, RetailPrice = 150F, WholeScalePrice = 85F, WriteOff = 0, Receipt = 0, Sold = 50 }
            }
        };

        await context.WorkShifts.AddRangeAsync(openedShiftId1, openedShiftId2);
        await context.SaveChangesAsync();
        await context.CloseWorkShiftAsync(id1, 150);

        Assert.That(context.WorkShifts.Count(), Is.EqualTo(1));
        Assert.That(context.WorkShifts.Any(shift => shift.IsOpened), Is.False);
    }

    [Test]
    [Description("Loading working shift for all users")]
    public async Task LoadingWorkingShiftsForPairUsersAsync()
    {
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var earlySuitableShiftCloseTime = DateTime.Parse("2000-01-01 12:00");
        var lateSuitableShiftCloseTime = DateTime.Parse("2000-01-02 03:00");
        await AddWorkShiftsAsync(context, earlySuitableShiftCloseTime, lateSuitableShiftCloseTime);
        var suitableShifts = await context.GetWorkShiftSnapshotsAsync(-1, DateOnly.Parse("2000-01-01"));
        Assert.That(suitableShifts.Count, Is.EqualTo(2));

        Assert.That(suitableShifts.First(c => c.CloseTime == earlySuitableShiftCloseTime).UserId, Is.EqualTo(1));
        Assert.That(suitableShifts.First(c => c.CloseTime == lateSuitableShiftCloseTime).UserId, Is.EqualTo(2));
        Assert.That(suitableShifts.Any(c => c.IsOpened), Is.False);
        Assert.That(suitableShifts.Any(c => c.CloseTime > DateTime.Parse("2000-01-02 06:00")), Is.False);
    }

    [Test]
    [Description("Loading working shift for target user")]
    public async Task LoadingWorkingShiftForTargetUser()
    {
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var earlySuitableShiftCloseTime = DateTime.Parse("2000-01-01 12:00");
        var lateSuitableShiftCloseTime = DateTime.Parse("2000-01-02 03:00");
        await AddWorkShiftsAsync(context, earlySuitableShiftCloseTime, lateSuitableShiftCloseTime);
        var suitableShifts = await context.GetWorkShiftSnapshotsAsync(1, DateOnly.Parse("2000-01-01"));
        Assert.That(suitableShifts.Count, Is.EqualTo(1));

        var targetShift = suitableShifts.Single();
        Assert.That(targetShift.UserId, Is.EqualTo(1));
        Assert.That(targetShift.CloseTime, Is.EqualTo(earlySuitableShiftCloseTime));
        Assert.That(targetShift.IsOpened, Is.False);
    }

    private async Task AddWorkShiftsAsync(PostgresProxy context, DateTime earlySuitableShiftCloseTime, DateTime lateSuitableShiftCloseTime)
    {
        var firstItemId = Guid.NewGuid();
        var secondItemId = Guid.NewGuid();
        #region Target day early closed working shift

        var earlySuitableShift = new WorkShift
        {
            Cash = 100,
            IsOpened = false,
            OpenTime = DateTime.Parse("2000-01-01 07:00"),
            CloseTime = earlySuitableShiftCloseTime,
            UserId = 1,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>
            {
                new()
                {
                    Id = firstItemId, RetailPrice = 100F, WholeScalePrice = 80F, WriteOff = 1, Receipt = 2, Sold = 3
                },
                new ()
                {
                    Id = secondItemId, RetailPrice = 150F, WholeScalePrice = 85F, WriteOff = 0, Receipt = 0, Sold = 5
                }
            }
        };

        #endregion

        #region Target day late closed working shift

        var lateSuitableShift = new WorkShift
        {
            Cash = 200,
            IsOpened = false,
            OpenTime = DateTime.Parse("2000-01-01 13:00"),
            CloseTime = lateSuitableShiftCloseTime,
            UserId = 2,
            UserDisplayName = "Second",
            GoodItemStates = new List<GoodsItemStorage>
            {
                new()
                {
                    Id = firstItemId, RetailPrice = 120F, WholeScalePrice = 85F, WriteOff = 1, Receipt = 2, Sold = 3
                },
                new ()
                {
                    Id = secondItemId, RetailPrice = 155F, WholeScalePrice = 95F, WriteOff = 0, Receipt = 0, Sold = 5
                }
            }
        };

        #endregion

        #region Next day closed working shift

        var earlyNotSuitableShift = new WorkShift
        {
            Cash = 200,
            IsOpened = false,
            OpenTime = DateTime.Parse("2000-01-02 04:00"),
            CloseTime = DateTime.Parse("2000-01-02 08:00"),
            UserId = 2,
            UserDisplayName = "Second",
            GoodItemStates = new List<GoodsItemStorage>
            {
                new() { Id = firstItemId, RetailPrice = 100F, WholeScalePrice = 80F, WriteOff = 1, Receipt = 2, Sold = 3 },
                new () { Id = secondItemId, RetailPrice = 150F, WholeScalePrice = 85F, WriteOff = 0, Receipt = 0, Sold = 5 }
            }
        };

        #endregion

        #region Target day opened working shift

        var notActiveShift = new WorkShift
        {
            Cash = 100,
            IsOpened = true,
            OpenTime = DateTime.Parse("2000-01-01 06:30"),
            CloseTime = DateTime.Parse("2000-01-01 06:35"),
            UserId = 1,
            UserDisplayName = "First",
            GoodItemStates = new List<GoodsItemStorage>
            {
                new()
                {
                    Id = firstItemId, RetailPrice = 100F, WholeScalePrice = 80F, WriteOff = 1, Receipt = 2, Sold = 3
                },
                new ()
                {
                    Id = secondItemId, RetailPrice = 150F, WholeScalePrice = 85F, WriteOff = 0, Receipt = 0, Sold = 5
                }
            }
        };

        #endregion

        await context.WorkShifts.AddRangeAsync(earlySuitableShift, lateSuitableShift, earlyNotSuitableShift, notActiveShift);
        await context.SaveChangesAsync();
    }
}