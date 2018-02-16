namespace Boondocks.Services.DataAccess.Domain
{
    using Interfaces;
    using Newtonsoft.Json;

    public abstract class NamedEntityBase : EntityBase, INamedEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}