using System.Data;

namespace Boondocks.Services.DataAccess.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}