using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.DataBase;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class AdminStorageDataBaseTests
{
    private static readonly DbContextOptions Options = new DbContextOptionsBuilder<PostgresProxy>()
        .UseInMemoryDatabase(databaseName: "goods_account")
        .EnableSensitiveDataLogging()
        .Options;

    [Test]
    [Description("Try to identify user")]
    public async Task ThrowNoCredentialExceptionTest()
    {
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(new User
        {
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Hash = Array.Empty<byte>(),
            Name = "123",
            Salt = "123",
            Surname = "123",
            PasswordExpired = DateTime.Now,
            Role = UserRole.Administrator,
            UserLogin = "123",
        });
        
        await context.Users.AddAsync(new User
        {
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Hash = Array.Empty<byte>(),
            Name = "123",
            Salt = "123",
            Surname = "123",
            PasswordExpired = DateTime.Now,
            Role = UserRole.RegisteredUser,
            UserLogin = "123",
        });
        
        Assert.That(context.Users.Any(), Is.False);

        await context.SaveChangesAsync();
        Assert.That(context.Users.Count(), Is.EqualTo(2));

        var targetId = (await context.Users.SingleAsync(user => user.Role == UserRole.RegisteredUser)).Id;
        await context.WorkShifts.AddAsync(new WorkShift
        {
            Cash = 0,
            GoodItemStates = new List<GoodsItemStorage>(),
            UserId = targetId,
            IsOpened = true,
            UserDisplayName = string.Empty,
            Comments = string.Empty
        });

        await context.SaveChangesAsync();
        Assert.ThrowsAsync<EntityNotFoundException>(async () => await context.UpdateGoodsStorageAsync(targetId, new List<Guid>(), new List<GoodsItem>()));
    }

    [Test]
    [Description("Try to get target working shift")]
    public async Task ThrowNoWorkingShiftTest()
    {
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(new User
        {
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Hash = Array.Empty<byte>(),
            Name = "123",
            Salt = "123",
            Surname = "123",
            PasswordExpired = DateTime.Now,
            Role = UserRole.Administrator,
            UserLogin = "123",
        });

        await context.SaveChangesAsync();
        var targetId = context.Users.Single().Id;

        Assert.That(context.WorkShifts.Any(), Is.False);
        await context.WorkShifts.AddAsync(new WorkShift
        {
            Cash = 0,
            GoodItemStates = new List<GoodsItemStorage>(),
            UserId = targetId + 1,
            IsOpened = true,
            UserDisplayName = "123",
            Comments = string.Empty
        });

        await context.WorkShifts.AddAsync(new WorkShift
        {
            Cash = 0,
            GoodItemStates = new List<GoodsItemStorage>(),
            UserId = targetId,
            IsOpened = false,
            UserDisplayName = "123",
            Comments = string.Empty
        });

        await context.SaveChangesAsync();
        Assert.That(context.WorkShifts.Count(), Is.EqualTo(2));
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await context.UpdateGoodsStorageAsync(targetId, new List<Guid>(), new List<GoodsItem>()));
    }

    [Test]
    [Description("Update item")]
    public async Task UpdateNewItemTest()
    {
        const int oldStorage = 10;
        const int newStorage = 20;

        const float oldRetailPrice = 100F;
        const float newRetailPrice = 200F;

        const float oldWholePrice = 80F;
        const float newWholePrice = 100F;

        const string oldName = "Item";
        const string newName = "New item";
        var id = Guid.NewGuid();
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Goods.AddAsync(new GoodsItem
        {
            Actives = true,
            CurrentItemsInStorageCount = oldStorage,
            Id = id,
            Name = oldName,
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice
        });

        await context.Users.AddAsync(new User
        {
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Hash = Array.Empty<byte>(),
            Name = "123",
            Salt = "123",
            Surname = "123",
            PasswordExpired = DateTime.Now,
            Role = UserRole.Administrator,
            UserLogin = "123",
        });

        await context.SaveChangesAsync();
        var targetId = context.Users.Single().Id;

        Assert.That(context.WorkShifts.Any(), Is.False);
        await context.WorkShifts.AddAsync(new WorkShift
        {
            Cash = 0,
            GoodItemStates = new List<GoodsItemStorage>(),
            UserId = targetId,
            IsOpened = true,
            UserDisplayName = "123",
            Comments = string.Empty
        });
        
        await context.SaveChangesAsync();
        await context.UpdateGoodsStorageAsync(
            targetId, 
            new List<Guid>(), 
            new List<GoodsItem>{ new()
            {
                CurrentItemsInStorageCount = newStorage,
                RetailPrice = newRetailPrice,
                WholeScalePrice = newWholePrice,
                Name = newName,
                Id = id
            } });

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Name, Is.EqualTo(newName));
        Assert.That(updatedItem.CurrentItemsInStorageCount, Is.EqualTo(newStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(newRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(newWholePrice));
    }

    [Test]
    [Description("Add item")]
    public async Task AddNewItemTest()
    {;
        const int newStorage = 20;
        const float newRetailPrice = 200F;
        const float newWholePrice = 100F;
        const string newName = "New item";
        var id = Guid.NewGuid();
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.Users.AddAsync(new User
        {
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Hash = Array.Empty<byte>(),
            Name = "123",
            Salt = "123",
            Surname = "123",
            PasswordExpired = DateTime.Now,
            Role = UserRole.Administrator,
            UserLogin = "123",
        });

        await context.SaveChangesAsync();
        var targetId = context.Users.Single().Id;

        Assert.That(context.WorkShifts.Any(), Is.False);
        await context.WorkShifts.AddAsync(new WorkShift
        {
            Cash = 0,
            GoodItemStates = new List<GoodsItemStorage>(),
            UserId = targetId,
            IsOpened = true,
            UserDisplayName = "123",
            Comments = string.Empty
        });

        await context.SaveChangesAsync();

        Assert.That(context.Goods.Any(), Is.False);
        await context.UpdateGoodsStorageAsync(
            targetId,
            new List<Guid>(),
            new List<GoodsItem>{ new()
            {
                CurrentItemsInStorageCount = newStorage,
                RetailPrice = newRetailPrice,
                WholeScalePrice = newWholePrice,
                Name = newName,
                Id = id
            } });

        Assert.That(context.Goods.Count(), Is.EqualTo(1));

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Name, Is.EqualTo(newName));
        Assert.That(updatedItem.CurrentItemsInStorageCount, Is.EqualTo(newStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(newRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(newWholePrice));
    }

    [Test]
    [Description("Remove item")]
    public async Task RemoveItemTest()
    {
        const int oldStorage = 10;
        const float oldRetailPrice = 100F;
        const float oldWholePrice = 80F;
        const string oldName = "Item";

        var id = Guid.NewGuid();
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Goods.AddAsync(new GoodsItem
        {
            Actives = true,
            CurrentItemsInStorageCount = oldStorage,
            Id = id,
            Name = oldName,
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice
        });

        await context.Users.AddAsync(new User
        {
            BirthDate = DateOnly.FromDateTime(DateTime.Now),
            Hash = Array.Empty<byte>(),
            Name = "123",
            Salt = "123",
            Surname = "123",
            PasswordExpired = DateTime.Now,
            Role = UserRole.Administrator,
            UserLogin = "123",
        });

        await context.SaveChangesAsync();
        var targetId = context.Users.Single().Id;

        Assert.That(context.WorkShifts.Any(), Is.False);
        await context.WorkShifts.AddAsync(new WorkShift
        {
            Cash = 0,
            GoodItemStates = new List<GoodsItemStorage>(),
            UserId = targetId,
            IsOpened = true,
            UserDisplayName = "123",
            Comments = string.Empty
        });

        await context.SaveChangesAsync();
        await context.UpdateGoodsStorageAsync(
            targetId,
            new List<Guid> { id },
            new List<GoodsItem>());

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.False);
        Assert.That(updatedItem.Name, Is.EqualTo(oldName));
        Assert.That(updatedItem.CurrentItemsInStorageCount, Is.EqualTo(oldStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(oldRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(oldWholePrice));
    }
}