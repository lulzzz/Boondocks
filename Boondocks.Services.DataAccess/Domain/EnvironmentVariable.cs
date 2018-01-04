namespace Boondocks.Services.DataAccess.Domain
{
    public abstract class EnvironmentVariable : EntityBase
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}