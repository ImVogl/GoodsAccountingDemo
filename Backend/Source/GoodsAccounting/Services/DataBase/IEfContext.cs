namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Common data base proxy class interface.
/// </summary>
public interface IEfContext : IUsersContext, IAdminStorageContext
{
    /// <summary>
    /// Recreation data base.
    /// </summary>
    public void RecreateDataBase();

}