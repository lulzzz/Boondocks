namespace Boondocks.Device.Infra.Repositories.Models
{
    public class NamedValueData<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }
}