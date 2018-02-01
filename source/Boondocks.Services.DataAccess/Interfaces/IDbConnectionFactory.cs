namespace Boondocks.Services.DataAccess.Interfaces
{
    using System.Data;

    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}