using NetFusion.Rest.Client.Resources;

namespace Boondocks.Auth.Tests.Resources
{
    /// <summary>
    /// Models the resource returned from the Authentication WebApi.
    /// </summary>
    public class AuthResultResource : HalResource
    {
        public string CorrelationId { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
