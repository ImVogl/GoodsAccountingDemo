using GoodsAccounting.Model;
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
        Assert.ThrowsAsync<TableAccessException>(async () => await context.UpdateGoodsStorageAsync(targetId, new Dictionary<Guid, GoodsItemStateChanging>()));
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
            await context.UpdateGoodsStorageAsync(targetId, new Dictionary<Guid, GoodsItemStateChanging>()));
    }

    [Test]
    [Description("Update item with setting items count in storage")]
    public async Task UpdateItemStorageTest()
    {
        const int oldStorage = 10;
        const int newStorage = 20;

        const float oldRetailPrice = 100F;
        const float newRetailPrice = 200F;

        const float oldWholePrice = 80F;
        const float newWholePrice = 100F;

        const string oldName = "Item category";
        const string newName = "New item category";
        var id = Guid.NewGuid();
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Goods.AddAsync(new GoodsItem
        {
            Actives = true,
            Storage = oldStorage,
            Id = id,
            Name = "Item",
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice,
            Category = oldName
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
            GoodItemStates = new List<GoodsItemStorage>
            {
                new (){ GoodsInStorage = oldStorage, Id = id, WholeScalePrice = oldWholePrice, RetailPrice = oldRetailPrice, WriteOff = 0, Receipt = 0, Sold = 0 }
            },
            UserId = targetId,
            IsOpened = true,
            UserDisplayName = "123",
            Comments = string.Empty
        });
        
        await context.SaveChangesAsync();
        await context.UpdateGoodsStorageAsync(
            targetId, 
            new Dictionary<Guid, GoodsItemStateChanging>
            {
                {
                    id, 
                    new GoodsItemStateChanging { 
                        Storage = newStorage,
                        WholeScalePrice = newWholePrice,
                        RetailPrice = newRetailPrice,
                        Category = newName,
                        Receipt = 1,
                        WriteOff = 1
                    }
                }
            });

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Category, Is.EqualTo(newName));
        Assert.That(updatedItem.Storage, Is.EqualTo(newStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(newRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(newWholePrice));

        Assert.That(context.WorkShifts.Any(), Is.True);
        var updatedHistory = await context.WorkShifts.SingleAsync();
        var targetItem = updatedHistory.GoodItemStates.Single(item => item.Id == id);
        Assert.That(targetItem.GoodsInStorage, Is.EqualTo(newStorage));
        Assert.That(targetItem.RetailPrice, Is.EqualTo(newRetailPrice));
        Assert.That(targetItem.WholeScalePrice, Is.EqualTo(newWholePrice));
        Assert.That(targetItem.Receipt, Is.GreaterThan(0));
        Assert.That(targetItem.WriteOff, Is.GreaterThan(0));
    }

    [Test]
    [Description("Update item with setting items count writing-off test")]
    public async Task UpdateItemMovingTest()
    {
        const int oldStorage = 10;

        const float oldRetailPrice = 100F;
        const float newRetailPrice = 200F;

        const float oldWholePrice = 80F;
        const float newWholePrice = 100F;

        const string oldName = "Item category";
        const string newName = "New item category";

        const int receipt = 10;
        const int writeOff = 5;
        var id = Guid.NewGuid();
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Goods.AddAsync(new GoodsItem
        {
            Actives = true,
            Storage = oldStorage,
            Id = id,
            Name = "Item",
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice,
            Category = oldName
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
            GoodItemStates = new List<GoodsItemStorage>
            {
                new (){ GoodsInStorage = oldStorage, Id = id, WholeScalePrice = oldWholePrice, RetailPrice = oldRetailPrice, WriteOff = 0, Receipt = 0, Sold = 0 }
            },
            UserId = targetId,
            IsOpened = true,
            UserDisplayName = "123",
            Comments = string.Empty
        });

        await context.SaveChangesAsync();
        await context.UpdateGoodsStorageAsync(
            targetId,
            new Dictionary<Guid, GoodsItemStateChanging>
            {
                {
                    id,
                    new GoodsItemStateChanging {
                        Storage = oldStorage,
                        WholeScalePrice = newWholePrice,
                        RetailPrice = newRetailPrice,
                        Category = newName,
                        Receipt = receipt,
                        WriteOff = writeOff
                    }
                }
            });

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Category, Is.EqualTo(newName));
        Assert.That(updatedItem.Storage, Is.EqualTo(oldStorage + receipt - writeOff));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(newRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(newWholePrice));

        Assert.That(context.WorkShifts.Any(), Is.True);
        var updatedHistory = await context.WorkShifts.SingleAsync();
        var targetItem = updatedHistory.GoodItemStates.Single(item => item.Id == id);
        Assert.That(targetItem.GoodsInStorage, Is.EqualTo(oldStorage + receipt - writeOff));
        Assert.That(targetItem.RetailPrice, Is.EqualTo(newRetailPrice));
        Assert.That(targetItem.WholeScalePrice, Is.EqualTo(newWholePrice));
        Assert.That(targetItem.Receipt, Is.EqualTo(receipt));
        Assert.That(targetItem.WriteOff, Is.EqualTo(writeOff));
    }

    [Test]
    [Description("Add item test")]
    public async Task AddNewItemTest()
    {
        const int newStorage = 20;
        const float newRetailPrice = 200F;
        const float newWholePrice = 100F;
        const string newName = "New item";
        const string newCategory = "New item category";
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
        await context.AddNewGoodsItemAsync(
            targetId,
            new GoodsItem { Name = newName, Category = newCategory, Id = id, RetailPrice = newRetailPrice, WholeScalePrice = newWholePrice, Storage = newStorage, Actives = true });

        Assert.That(context.Goods.Count(), Is.EqualTo(1));

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Name, Is.EqualTo(newName));
        Assert.That(updatedItem.Category, Is.EqualTo(newCategory));
        Assert.That(updatedItem.Storage, Is.EqualTo(newStorage));
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
            Storage = oldStorage,
            Id = id,
            Name = oldName,
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice,
            Category = "123"
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
        await context.RemoveGoodsItemAsync(targetId, id);

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.False);
        Assert.That(updatedItem.Name, Is.EqualTo(oldName));
        Assert.That(updatedItem.Storage, Is.EqualTo(oldStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(oldRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(oldWholePrice));
    }

    [Test]
    [Description("Restore item")]
    public async Task RestoreItemTest()
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
            Actives = false,
            Storage = oldStorage,
            Id = id,
            Name = oldName,
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice,
            Category = "123"
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
        await context.RestoreGoodsItemAsync(targetId, id);

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Name, Is.EqualTo(oldName));
        Assert.That(updatedItem.Storage, Is.EqualTo(oldStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(oldRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(oldWholePrice));
    }


    [Test]
    [Description("Rename item")]
    public async Task RenameItemTest()
    {
        const int oldStorage = 10;
        const float oldRetailPrice = 100F;
        const float oldWholePrice = 80F;
        const string oldName = "Item";
        const string newName = "Item name";

        var id = Guid.NewGuid();
        await using var context = new PostgresProxy(Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Goods.AddAsync(new GoodsItem
        {
            Actives = true,
            Storage = oldStorage,
            Id = id,
            Name = oldName,
            RetailPrice = oldRetailPrice,
            WholeScalePrice = oldWholePrice,
            Category = "123"
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
        await context.RenameGoodsItemAsync(targetId, id, newName);

        var updatedItem = context.Goods.Single();
        Assert.That(updatedItem.Actives, Is.True);
        Assert.That(updatedItem.Name, Is.EqualTo(newName));
        Assert.That(updatedItem.Storage, Is.EqualTo(oldStorage));
        Assert.That(updatedItem.RetailPrice, Is.EqualTo(oldRetailPrice));
        Assert.That(updatedItem.WholeScalePrice, Is.EqualTo(oldWholePrice));
    }
}