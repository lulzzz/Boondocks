namespace Boondocks.Services.DataAccess.Interfaces
{
    public interface IBlobDataAccesConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}