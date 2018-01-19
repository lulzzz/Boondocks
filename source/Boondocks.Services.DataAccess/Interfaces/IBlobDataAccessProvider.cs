namespace Boondocks.Services.DataAccess.Interfaces
{
    public interface IBlobDataAccessProvider
    {
        IBlobDataAccess ApplicationVersionImages { get; }

        IBlobDataAccess SupervisorVersionImages { get; }
    }
}