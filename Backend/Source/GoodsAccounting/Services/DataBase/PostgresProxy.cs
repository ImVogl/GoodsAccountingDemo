using GoodsAccounting.Model;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
        var currentShift = await WorkShifts
            .Include(shift => shift.GoodItemStates)
            .SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId)
            .ConfigureAwait(false);

        if (currentShift == null)
            throw new EntityNotFoundException();

        foreach (var state in currentShift.GoodItemStates.Where(state => soldGoods.ContainsKey(state.Id)))
            state.Sold += soldGoods[state.Id];
        
        foreach(var item in Goods.Where(i => i.Actives))
            if (soldGoods.ContainsKey(item.Id))
                item.Storage -= soldGoods[item.Id];

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

        var goods = await Goods.Where(item => item.Actives).ToListAsync().ConfigureAwait(false);
        var states = goods.Select(item => new GoodsItemStorage
        {
            Id = item.Id,
            Sold = 0,
            RetailPrice = item.RetailPrice,
            Receipt = 0,
            WholeScalePrice = item.WholeScalePrice,
            WriteOff = 0
        }).ToList();
        
        var currentShift = new WorkShift
        {
            IsOpened = true,
            OpenTime = DateTime.Now,
            Cash = 0,
            UserId = id,
            UserDisplayName = $"{user.Surname} {(user.Name.Length > 1 ? user.Name[0] : string.Empty)}",
            Comments = string.Empty,
            GoodItemStates = states
        };
        
        await WorkShifts.AddAsync(currentShift).ConfigureAwait(false);
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CloseWorkShiftAsync(int id,int cash, string comment)
    {
        var shift = await WorkShifts
            .Include(shift => shift.GoodItemStates)
            .SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == id)
            .ConfigureAwait(false);

        if (shift == null)
            throw new EntityNotFoundException();
        
        shift.CloseTime = DateTime.Now;
        shift.IsOpened = false;
        shift.Cash = cash;
        if (!string.IsNullOrWhiteSpace(comment))
            shift.Comments = $"{shift.Comments}{Environment.NewLine}{comment}";

        var storage = Goods.ToDictionary(item => item.Id, item => item.Storage);
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
        var shifts = id < 1
            ? WorkShifts.Include(shift => shift.GoodItemStates)
            : WorkShifts.Where(shift => shift.UserId == id).Include(shift => shift.GoodItemStates);

        var dateShifts = shifts.Where(shift => (shift.CloseTime > lowDateTime && shift.CloseTime < heightDateTime) || shift.IsOpened);
        return await dateShifts.ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateGoodsStorageAsync(int userId, Dictionary<Guid, GoodsItemStateChanging> changing)
    {
        if (!await IsUserAdminAsync(userId).ConfigureAwait(false))
            throw new TableAccessException();

        var workingShift = await WorkShifts
            .Include(shift => shift.GoodItemStates)
            .SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId)
            .ConfigureAwait(false);

        if (workingShift == null)
            throw new EntityNotFoundException();
        
        foreach (var item in Goods)
        {
            if (!changing.ContainsKey(item.Id))
                continue;

            var diff = changing[item.Id].Storage != -1
                ? changing[item.Id].Storage - item.Storage 
                : 0;

            item.Storage = diff == 0 ? item.Storage - changing[item.Id].WriteOff + changing[item.Id].Receipt : changing[item.Id].Storage;
            item.WholeScalePrice = changing[item.Id].WholeScalePrice <= float.Epsilon ? item.WholeScalePrice : changing[item.Id].WholeScalePrice;
            item.RetailPrice = changing[item.Id].RetailPrice <= float.Epsilon ? item.RetailPrice : changing[item.Id].RetailPrice;
            item.Category = string.IsNullOrWhiteSpace(changing[item.Id].Category) ? item.Category : changing[item.Id].Category;
        }

        foreach (var item in workingShift.GoodItemStates)
        {
            if (!changing.ContainsKey(item.Id))
                continue;

            var diff = changing[item.Id].Storage != -1
                ? changing[item.Id].Storage - item.GoodsInStorage
                : 0;

            item.GoodsInStorage = diff == 0 ? item.GoodsInStorage - changing[item.Id].WriteOff + changing[item.Id].Receipt : changing[item.Id].Storage;
            item.WholeScalePrice = changing[item.Id].WholeScalePrice <= float.Epsilon ? item.WholeScalePrice : changing[item.Id].WholeScalePrice;
            item.RetailPrice = changing[item.Id].RetailPrice <= float.Epsilon ? item.RetailPrice : changing[item.Id].RetailPrice;
            item.WriteOff = changing[item.Id].WriteOff;
            item.Receipt = changing[item.Id].Receipt;
        }

        workingShift.Comments = AddCommentSourceMessage(workingShift.Comments, "Задано новое состояние для хранилища товаров");
        await SaveChangesAsync().ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    public async Task RenameGoodsItemAsync(int userId, Guid id, string newName)
    {

        if (!await IsUserAdminAsync(userId).ConfigureAwait(false))
            throw new TableAccessException();

        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId).ConfigureAwait(false);
        if (shift == null)
            throw new EntityNotFoundException();

        var item = await Goods.SingleOrDefaultAsync(item => item.Id == id).ConfigureAwait(false);
        if (item == null)
            throw new EntityNotFoundException();

        shift.Comments = AddCommentSourceMessage(shift.Comments, $"Товар переименован {item.Name} на {newName}");
        item.Name = newName;
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AddNewGoodsItemAsync(int userId, GoodsItem newItem)
    {
        if (!await IsUserAdminAsync(userId).ConfigureAwait(false))
            throw new TableAccessException();

        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId).ConfigureAwait(false);
        if (shift == null)
            throw new EntityNotFoundException();

        if (await Goods.AnyAsync(item => item.Name == newItem.Name).ConfigureAwait(false))
            throw new EntityExistsException();

        await Goods.AddAsync(newItem).ConfigureAwait(false);
        shift.Comments = AddCommentSourceMessage(shift.Comments, $"Добавлен товар с именем {newItem.Name} добавлен");
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RemoveGoodsItemAsync(int userId, Guid id)
    {
        if (!await IsUserAdminAsync(userId).ConfigureAwait(false))
            throw new TableAccessException();

        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId).ConfigureAwait(false);
        if (shift == null)
            throw new EntityNotFoundException();

        var item = await Goods.SingleAsync(item => item.Id == id).ConfigureAwait(false);
        if (item == null)
            throw new EntityNotFoundException();

        item.Actives = false;
        shift.Comments = AddCommentSourceMessage(shift.Comments, $"Товар {item.Name} выведен из продажи");
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RestoreGoodsItemAsync(int userId, Guid id)
    {
        if (!await IsUserAdminAsync(userId).ConfigureAwait(false))
            throw new TableAccessException();

        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.IsOpened && shift.UserId == userId).ConfigureAwait(false);
        if (shift == null)
            throw new EntityNotFoundException();

        var item = await Goods.SingleAsync(item => item.Id == id).ConfigureAwait(false);
        if (item == null)
            throw new EntityNotFoundException();

        item.Actives = true;
        shift.Comments = AddCommentSourceMessage(shift.Comments, $"Товар {item.Name} возвращен в продажу");
        await SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> GetWorkingShiftStateAsync(int userId)
    {
        if (!await Users.AnyAsync(user => user.Id == userId).ConfigureAwait(false))
            return false;

        var shift = await WorkShifts.SingleOrDefaultAsync(shift => shift.UserId == userId && shift.IsOpened).ConfigureAwait(false);
        return shift != null;
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
    public async Task RemoveUserAsync(int id)
    {
        var user = await Users.SingleOrDefaultAsync(user => user.Id == id).ConfigureAwait(false);
        if (user == null)
            return;
        
        Users.Remove(user);
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
        modelBuilder.Entity<User>().Property(user => user.Id)
            .ValueGeneratedOnAdd()
            .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

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
        modelBuilder.Entity<GoodsItem>().Property(item => item.Storage).IsRequired();

        modelBuilder.Entity<WorkShift>().HasIndex(shift => shift.Index).IsUnique();
        modelBuilder.Entity<WorkShift>().Property(shift => shift.Index)
            .ValueGeneratedOnAdd()
            .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

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
            .HasForeignKey("ShiftIdentifier")
            .IsRequired();

        modelBuilder.Entity<GoodsItemStorage>().HasIndex(storage => storage.Index).IsUnique();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Index)
            .ValueGeneratedOnAdd()
            .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);


        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Id).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.GoodsInStorage).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Receipt).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.RetailPrice).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.Sold).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.WholeScalePrice).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property(storage => storage.WriteOff).IsRequired();
        modelBuilder.Entity<GoodsItemStorage>().Property("ShiftIdentifier").HasColumnName("shift_identifier").IsRequired();
    }

    /// <summary>
    /// Checking user admin role access.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>Value is indicating that user exists and has admin role.</returns>
    private async Task<bool> IsUserAdminAsync(int id)
    {
        var user = await Users.SingleOrDefaultAsync(user => user.Id == id && user.Role == UserRole.Administrator)
            .ConfigureAwait(false);

        return user != null;
    }

    /// <summary>
    /// Adding message to source.
    /// </summary>
    /// <param name="source">Source message.</param>
    /// <param name="additional">Additional message.</param>
    /// <returns>Concatenated message.</returns>
    private string AddCommentSourceMessage(string source, string additional)
    {
        return string.IsNullOrWhiteSpace(source)
            ? $"{DateTime.Now:g}: {additional}"
            : $"{source}{Environment.NewLine}{DateTime.Now:g}: {additional}";
    }
}