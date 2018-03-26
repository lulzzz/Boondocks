namespace Boondocks.Auth.Api.Models
{
    /// <summary>
    /// Information about the resource for which authentication is
    /// being requested.
    /// </summary>
    public class AuthResourceModel
    {
        /// <summary>
        /// Identifies the service owning the  resources for which
        /// access is being requested (i.e. Docker Registry).
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Details about the access being requested for each resource.
        /// </summary>
        public string[] Scope { get; set; }

        // Indicates that the submitted authentication is for a set of resources.
        public bool IsResourceAccessRequest =>
            !string.IsNullOrEmpty(Service) && Scope != null;
    }
}
