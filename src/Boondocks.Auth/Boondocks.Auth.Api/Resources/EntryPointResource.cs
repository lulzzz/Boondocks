using NetFusion.Rest.Resources.Hal;

namespace Boondocks.Auth.Api.Resources
{
    /// <summary>
    /// Resource containing the entry-point URIs for the microservice.
    /// </summary>
    public class EntryPointResource : HalEntryPointResource
    {
        public EntryPointResource()
        {
            Version = typeof(EntryPointResource).Assembly
                .GetName().Version.ToString();   
        }
    }
}