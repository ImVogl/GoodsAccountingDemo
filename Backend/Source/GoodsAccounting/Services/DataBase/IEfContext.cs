namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Common data base proxy class interface.
/// </summary>
public interface IEfContext : IUsersContext, IStorageContext
{
    /// <summary>
    /// Recreation data base.
    /// </summary>
    public void RecreateDataBase();

}