using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Proxy for PosrgreSQL data base.
/// </summary>
public class PostgresProxy : DbContext, IEfContext
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
    public DbSet<GoodsItem> Goods { get; set; } = null!;

    /// <inheritdoc />
    public DbSet<WorkShift> WorkShifts { get; set; } = null!;
    
    /// <inheritdoc />
    public void RecreateDataBase()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    /// <inheritdoc />
    public async Task UpdateSoldGoodsAsync(int userId, Dictionary<Guid, int> soldGoods)
    {
        var currentShift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId).ConfigureAwait(false);
        if (currentShift == null)
            throw new EntityNotFoundException();

        foreach (var state in currentShift.GoodItemStates.Where(state => soldGoods.ContainsKey(state.Id)))
            state.Sold += soldGoods[state.Id];
        
        foreach(var item in Goods.Where(i => i.Actives && soldGoods.ContainsKey(i.Id)))
            item.CurrentItemsInStorageCount -= soldGoods[item.Id];

        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async  Task InitWorkShiftAsync(int id)
    {
        if (await WorkShifts.AnyAsync(shift => shift.IsOpened && shift.UserId == id).ConfigureAwait(false))
            throw new EntityExistsException();

        var user = await Users.SingleOrDefaultAsync(user => user.Id == id).ConfigureAwait(false);
        if (user == null)
            throw new EntityNotFoundException();
        
        var goods = await Goods.ToListAsync().ConfigureAwait(false);
        var currentShift = new WorkShift
        {
            OpenTime = DateTime.Now,
            Cash = 0,
            UserId = id,
            UserDisplayName = $"{user.Surname} {(user.Name.Length > 1 ? user.Name[0] : string.Empty)}",
            Comments = string.Empty,
            GoodItemStates = goods.Where(item => item.Actives).Select(item => new GoodsItemStorage
            {
                Id = item.Id,
                Sold = 0,
                RetailPrice = item.RetailPrice,
                Receipt = 0,
                WholeScalePrice = item.WholeScalePrice,
                WriteOff = 0
            }).ToList()
        };

        await WorkShifts.AddAsync(currentShift).ConfigureAwait(false);
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CloseWorkShiftAsync(int id,int cash)
    {
        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == id).ConfigureAwait(false);
        if (shift == null)
            throw new EntityNotFoundException();
        
        shift.CloseTime = DateTime.Now;
        shift.IsOpened = false;
        shift.Cash = cash;
        var changes = shift.GoodItemStates.ToDictionary(item => item.Id, item => item);
        foreach (var item in Goods.Where(item => changes.ContainsKey(item.Id)))
        {
            item.CurrentItemsInStorageCount += changes[item.Id].Receipt - changes[item.Id].WriteOff;
            item.RetailPrice = changes[item.Id].RetailPrice;
            item.WholeScalePrice = changes[item.Id].WholeScalePrice;
        }

        var storage = Goods.ToDictionary(item => item.Id, item => item.CurrentItemsInStorageCount);
        foreach (var item in shift.GoodItemStates.Where(item => storage.ContainsKey(item.Id)))
            item.GoodsInStorage = storage[item.Id];

        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IList<WorkShift>> GetWorkShiftSnapshotsAsync(int id, DateOnly date)
    {
        // We decided that all working shifts, that were closed till 6 am, refer to the previous day.
        var lowDateTime = date.ToDateTime(new TimeOnly(6, 0));
        var heightDateTime = date.AddDays(1).ToDateTime(new TimeOnly(6, 0));
        return id < 1 
            ? await WorkShifts
                .Where(shift => !shift.IsOpened && shift.CloseTime > lowDateTime && shift.CloseTime < heightDateTime)
                .Include(shift => shift.GoodItemStates)
                .ToListAsync().ConfigureAwait(false)
            : await WorkShifts
                .Where(shift => !shift.IsOpened && shift.CloseTime > lowDateTime && shift.CloseTime < heightDateTime && shift.UserId == id)
                .Include(shift => shift.GoodItemStates)
                .ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateGoodsStorageAsync(int userId, List<Guid> remove, IList<GoodsItem> goods)
    {
        var user = await Users.SingleOrDefaultAsync(user => user.Id == userId && user.Role == UserRole.Administrator)
            .ConfigureAwait(false);

        if (user == null)
            throw new EntityNotFoundException();

        var workingShift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId).ConfigureAwait(false);
        if (workingShift == null)
            throw new EntityNotFoundException();

        var message = $"{DateTime.Now:g}Пользователь {user.UserLogin} задал новое состояние для хранилища товаров;";
        workingShift.Comments = string.IsNullOrWhiteSpace(workingShift.Comments)
            ? message
            : $"{workingShift.Comments}{Environment.NewLine}{message}";

        var grouped = goods.ToDictionary(item => item.Id, item => item);
        foreach (var item in Goods.Where(item => grouped.ContainsKey(item.Id)))
        {
            item.CurrentItemsInStorageCount = grouped[item.Id].CurrentItemsInStorageCount;
            item.Name = grouped[item.Id].Name;
            item.WholeScalePrice = grouped[item.Id].WholeScalePrice;
            item.RetailPrice = grouped[item.Id].RetailPrice;
            item.Actives = true;
        }

        foreach (var item in Goods.Where(item => remove.Contains(item.Id)))
            item.Actives = false;

        foreach (var id in grouped.Keys)
        {
            if (!await Goods.AllAsync(item => item.Id != id).ConfigureAwait(false))
                continue;

            grouped[id].Actives = true;
            await Goods.AddAsync(grouped[id]).ConfigureAwait(false);
        }

        await SaveChangesAsync().ConfigureAwait(false);
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
        modelBuilder.Entity<User>().Property(user => user.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<User>().Property(user => user.UserLogin).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Name).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Surname).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.BirthDate).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Role).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Hash).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.PasswordExpired).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Salt).IsRequired();

        modelBuilder.Entity<GoodsItem>().HasIndex(item => item.Id).IsUnique();
        modelBuilder.Entity<GoodsItem>().Property(item => item.Name).IsRequired();
        modelBuilder.Entity<GoodsItem>().Property(item => item.Actives).IsRequired();
        modelBuilder.Entity<GoodsItem>().Property(item => item.RetailPrice).IsRequired();
        modelBuilder.Entity<GoodsItem>().Property(item => item.WholeScalePrice).IsRequired();
        modelBuilder.Entity<GoodsItem>().Property(item => item.CurrentItemsInStorageCount).IsRequired();

        modelBuilder.Entity<WorkShift>().HasIndex(shift => shift.Index).IsUnique();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.Index).ValueGeneratedOnAdd();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.OpenTime).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.CloseTime).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.Cash).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.UserId).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.UserDisplayName).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.IsOpened).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.Comments).IsRequired();
        modelBuilder.Entity<WorkShift>()
            .HasMany(shift => shift.GoodItemStates)
            .WithOne()
            .HasPrincipalKey(shift => shift.Index)
            .IsRequired();

        modelBuilder.Entity<GoodsItemStorage>().HasIndex(storage => storage.Index).IsUnique();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Index).ValueGeneratedOnAdd();
        
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Id).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.GoodsInStorage).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Receipt).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.RetailPrice).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Sold).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.WholeScalePrice).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.WriteOff).IsRequired();
    }
}