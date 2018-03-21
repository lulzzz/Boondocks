using NetFusion.Rest.Resources.Hal;

namespace Boondocks.Auth.Api.Resources
{
    /// <summary>
    /// Resource returned from the WebApi containing the access permissions
    /// Allowed for an authenticated identity.
    /// </summary>
    public class AccessResource : HalResource
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string[] Actions {get; set; }
    }
}