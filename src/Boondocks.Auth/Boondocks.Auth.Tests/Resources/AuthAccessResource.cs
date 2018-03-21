using NetFusion.Rest.Client.Resources;

namespace Boondocks.Auth.Tests.Resources
{
    /// <summary>
    /// Models the returned resource from the authentication WebApi
    /// Containing the actions for a given resource.
    /// </summary>
    public class AuthAccessResource : HalResource
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string[] Actions { get; set; }
    }
}
