using Microsoft.Extensions.Logging;

namespace Boondocks.Auth.App
{
    /// <summary>
    /// Constants identifying log categories.
    /// </summary>
    public class LogEvents
    {
        // Base value for Boondocks Authentication logs:
        private const int PluginLog = 100;

        // Error log event values:
        public static EventId ProviderRegistrationError = new EventId(-(PluginLog + 2), "Security token validation error");
        public static EventId RequiredCredentialsError = new EventId(-(PluginLog + 3), "Required credential values not specified");
        public static EventId UnknownRequestingService = new EventId(-(PluginLog + 4), "Unknown service requesting authentication token");
        public static EventId InvalidProviderResult = new EventId(-(PluginLog + 5), "Invalid authentication provider result returned");
        public static EventId TokenValidationError = new EventId(-(PluginLog + 6), "Security Token Validation Error");
        public static EventId UnexpectedAuthError = new EventId(-(PluginLog + 8), "Unexpected Authentication Error");

        // Detail log event values:
        public static EventId CertFileLoaded = new EventId(PluginLog + 20, "Certificate file has been loaded");
        public static EventId AuthRequestReceived = new EventId(PluginLog + 30, "Authentication request received from service");
        public static EventId AuthProviderResolved = new EventId(PluginLog + 31, "Authentication provider resolved for service");
        public static EventId AuthResultDetermined = new EventId(PluginLog + 32, "Authentication result determined.");
        public static EventId DeviceAuthKeyNotFound = new EventId(PluginLog + 33, "Authentication key not found for device.");
    }
}