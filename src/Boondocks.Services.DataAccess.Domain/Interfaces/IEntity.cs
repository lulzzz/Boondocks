namespace Boondocks.Services.DataAccess.Domain.Interfaces
{
    using System;

    public interface IEntity 
    {
        Guid Id { get; }

        DateTime CreatedUtc { get; }
    }

    public interface INamedEntity : IEntity
    {
        string Name { get; }
    }
}