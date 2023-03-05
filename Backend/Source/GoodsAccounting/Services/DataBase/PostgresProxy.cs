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
    public async Task UpdateSoldGoodsAsync(Dictionary<Guid, int> soldGoods)
    {
        var currentShift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened).ConfigureAwait(false);
        if (currentShift == null)
            throw new EntityNotFoundException();

        foreach (var state in currentShift.GoodItemStates)
        {
            if (!soldGoods.ContainsKey(state.Id))
                continue;

            state.Sold += soldGoods[state.Id];
            state.GoodsInStorage -= soldGoods[state.Id];
        }

        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async  Task InitWorkShiftAsync(int id)
    {
        if (await WorkShifts.AnyAsync(shift => shift.IsOpened).ConfigureAwait(false))
            throw new EntityExistsException();
        
        var lastShift = await WorkShifts.LastOrDefaultAsync().ConfigureAwait(false);
        var states = lastShift?.GoodItemStates.ToDictionary(item => item.Id, item => item.GoodsInStorage);
        var receipt = lastShift?.GoodItemStates.ToDictionary(item => item.Id, item => item.Receipt);
        var writeOff = lastShift?.GoodItemStates.ToDictionary(item => item.Id, item => item.WriteOff);

        var user = await Users.SingleOrDefaultAsync(user => user.Id == id).ConfigureAwait(false);
        var goods = await Goods.ToListAsync().ConfigureAwait(false);
        var currentShift = new WorkShift
        {
            OpenTime = DateTime.Now,
            Cash = 0,
            UserId = id,
            UserDisplayName = user == null ? "Not identified"  : $"{user.Surname} {(user.Name.Length > 1 ? user.Name[0] : string.Empty)}",
            GoodItemStates = goods.Where(item => item.Actives).Select(item => new GoodsItemStorage
            {
                Id = item.Id,
                Sold = 0,
                GoodsInStorage = states != null && states.ContainsKey(item.Id) ? states[item.Id] : 0,
                RetailPrice = item.RetailPrice,
                Receipt = receipt != null && receipt.ContainsKey(item.Id) ? receipt[item.Id] : 0,
                WholeScalePrice = item.WholeScalePrice,
                WriteOff = writeOff != null && writeOff.ContainsKey(item.Id) ? writeOff[item.Id] : 0
            }).ToList()
        };

        await WorkShifts.AddAsync(currentShift).ConfigureAwait(false);
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CloseWorkShiftAsync(int id,int cash)
    {
        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened).ConfigureAwait(false);
        if (shift == null)
            throw new EntityNotFoundException();

        if (shift.UserId != id)
            throw new ArgumentException();

        shift.CloseTime = DateTime.Now;
        shift.IsOpened = false;
        shift.Cash = cash;
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

        modelBuilder.Entity<WorkShift>().HasIndex(shift => shift.OpenTime).IsUnique();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.CloseTime).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.Cash).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.UserId).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.UserDisplayName).IsRequired();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.IsOpened).IsRequired();
        modelBuilder.Entity<WorkShift>()
            .HasMany(shift => shift.GoodItemStates)
            .WithOne()
            .HasForeignKey(shift => shift.Index)
            .IsRequired();

        modelBuilder.Entity<GoodsItemStorage>().HasIndex(storage => storage.Index).IsUnique();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Index).ValueGeneratedOnAdd();

        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.GoodsInStorage).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Id).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Receipt).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.RetailPrice).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Sold).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.WholeScalePrice).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.WriteOff).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>()
            .HasOne(state => state.ParentShift)
            .WithMany()
            .IsRequired();
    }
}