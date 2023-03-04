using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Proxy for PosrgreSQL data base.
/// </summary>
public class PostgresProxy : DbContext, IDataBase
{
    /// <summary>
    /// Instancing new object of <see cref="PostgresProxy"/>.
    /// </summary>
    /// <param name="options"><see cref="DbContextOptions"/>.</param>
    public PostgresProxy(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public DbSet<User> Users { get; set; } = null!;

    /// <inheritdoc />
    public void RecreateDataBase()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(int id, string salt, byte[] hash)
    {
        var targetUser = await Users.SingleOrDefaultAsync(u => u.Id == id).ConfigureAwait(false);
        if (targetUser == null)
            throw new EntityNotFoundException();

        targetUser.Hash = hash;
        targetUser.Salt = salt;
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AddUserAsync(User user)
    {
        if (await DoesUserExistsAsync(user.UserLogin, user.Name, user.Surname, user.BirthDate).ConfigureAwait(false))
            throw new EntityExistsException();

        await Users.AddAsync(user).ConfigureAwait(false);
        await SaveChangesAsync().ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    public async Task<bool> DoesUserExistsAsync(string login, string name, string surname, DateOnly birthDay)
    {
        return await Users.AnyAsync(u =>
                login == u.UserLogin
                && birthDay == u.BirthDate
                && name == u.Name
                && surname == u.Surname)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(user => user.Id).IsUnique();
        modelBuilder.Entity<User>().Property(f => f.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<User>().Property(f => f.UserLogin).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.Name).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.Surname).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.BirthDate).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.Role).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.Hash).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.PasswordExpired).IsRequired();
        modelBuilder.Entity<User>().Property(f => f.Salt).IsRequired();
    }
}