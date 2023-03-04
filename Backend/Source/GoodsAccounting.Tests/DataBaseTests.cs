using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.DataBase;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class DataBaseTests
{
    private const string Login = "login";
    private const string Name = "name";
    private const string Surname = "surname";
    private const string Salt = "3cEPXowvwUyeXrERR+Bmzw==";
    private static readonly byte[] HashCode = new byte[]
    {
        236, 241, 119, 4, 227, 152, 147, 88, 5, 102, 69, 178, 84, 222, 162, 125, 98, 199, 204, 17, 245, 33, 112, 249,
        200, 70, 161, 122, 26, 169, 200, 228
    };
    private static readonly DateOnly BirthDay = DateOnly.Parse("1990-05-16");
    private static readonly User User = new User
    {
        BirthDate = BirthDay,
        Name = Name,
        Surname = Surname,
        UserLogin = Login,
        Hash = HashCode,
        Role = "role",
        Salt = Salt
    };

    private static readonly DbContextOptions Options = new DbContextOptionsBuilder<PostgresProxy>().UseInMemoryDatabase(databaseName: "goods_account").Options;
    private PostgresProxy _dataBase = null!;

    [SetUp]
    public void SetUp()
    {
        _dataBase = new PostgresProxy(Options);
    }

    [TearDown]
    public void TearDown()
    {
        _dataBase.Database.EnsureDeleted();
        _dataBase.Database.EnsureCreated();
        _dataBase.Dispose();
    }

    [Test]
    [Description("Pass table recreation")]
    public void TableRecreateTest()
    {
        _dataBase.RecreateDataBase();
    }

    [Test]
    [Description("Checking user exists in database.")]
    public async Task CheckUserInDbTest()
    {
        const string anyField = "user";
        Assert.That(_dataBase.Users, Is.Empty);

        _dataBase.Users.Add(User);
        Update();

        Assert.That(_dataBase.Users.Count(), Is.EqualTo(1));
        Assert.That(await _dataBase.DoesUserExistsAsync(Login, Name, Surname, BirthDay), Is.True);
        Assert.That(await _dataBase.DoesUserExistsAsync(anyField, Name, Surname, BirthDay), Is.False);
        Assert.That(await _dataBase.DoesUserExistsAsync(Login, anyField, Surname, BirthDay), Is.False);
        Assert.That(await _dataBase.DoesUserExistsAsync(Login, Name, anyField, BirthDay), Is.False);
        Assert.That(await _dataBase.DoesUserExistsAsync(Login, Name, Surname, DateOnly.Parse("1995-05-16")), Is.False);
    }

    [Test]
    [Description("Adding user test")]
    public async Task AddNewUserTest()
    {
        Assert.That(_dataBase.Users, Is.Empty);
        await _dataBase.AddUserAsync(User);

        Assert.That(_dataBase.Users, Is.Not.Empty);
        Assert.ThrowsAsync<EntityExistsException>(async () => await _dataBase.AddUserAsync(User));
    }

    [Test]
    [Description("Test password changing")]
    public async Task ChangePasswordTest()
    {
        const string newSalt = "UWn+I9UDkka/lpFL48uqXg==";
        var newHash = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Assert.That(_dataBase.Users, Is.Empty);

        _dataBase.Users.Add(User);
        Update();

        var user = await _dataBase.Users.SingleOrDefaultAsync().ConfigureAwait(false);
        if (user == null)
            Assert.Fail();

#pragma warning disable CS8602
        await _dataBase.ChangePasswordAsync(user.Id, newSalt, newHash).ConfigureAwait(false);
        user = await _dataBase.Users.SingleOrDefaultAsync().ConfigureAwait(false);
        if (user == null)
            Assert.Fail();
        
        Assert.That(user.Salt, Is.Not.EqualTo(User.Salt));
        var count = 0;
        for (var i = 0; i < User.Hash.Length; ++i)
        {
            if (User.Hash[i] != user.Hash[i])
            {
                Assert.Pass();
                break;
            }

            count++;
        }

        Assert.That(count, Is.Not.EqualTo(User.Hash.Length));
#pragma warning restore CS8602

        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _dataBase.ChangePasswordAsync(100, "123", new byte[] { 1, 2, 3 }));
    }

    private void Update()
    {
        _dataBase.SaveChanges();
        _dataBase.Dispose();
        _dataBase = new PostgresProxy(Options);
    }
}