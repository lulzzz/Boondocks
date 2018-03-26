using Autofac;
using Boondocks.Auth.Api.Commands;
using Boondocks.Auth.App.Modules;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;
using Microsoft.Extensions.Logging;
using NetFusion.Base.Validation;
using NetFusion.Bootstrap.Logging;
using NetFusion.Bootstrap.Validation;
using NetFusion.Messaging;
using System;
using System.Threading.Tasks;

namespace Boondocks.Auth.App.Ports
{
    /// <summary>
    /// Port invoked when an authentication command is received.  
    /// </summary>
    public class AuthenticationPort : IMessageConsumer
    {
        private ILogger<AuthenticationPort> _logger;
        private ILifetimeScope _lifetimeScope;
        private IAuthProviderModule _providerModule;
        private IValidationService _validationSrv;
        private ITokenService _tokenSrv;

        public AuthenticationPort(
            ILogger<AuthenticationPort> logger,
            ILifetimeScope lifetimeScope,
            IAuthProviderModule providerModule,
            IValidationService validationSrv,
            ITokenService tokenSrv)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            _providerModule = providerModule ?? throw new ArgumentNullException(nameof(providerModule));
            _validationSrv = validationSrv ?? throw new ArgumentNullException(nameof(validationSrv));
            _tokenSrv = tokenSrv ?? throw new ArgumentNullException(nameof(tokenSrv));
        }

        [InProcessHandler]
        public async Task<AuthResult> AuthenticateForService(AuthenticateCaller authCommand)
        {           
            // Validate the submitted credentials required from all providers:
            ValidationResultSet validationResults = _validationSrv.Validate(authCommand);
            if (validationResults.IsInvalid)
            {
                _logger.LogErrorDetails(
                    LogEvents.RequiredCredentialsError, 
                    "Invalid submitted context and/or credentials", validationResults);

                return AuthResult.Failed("Invalid authentication context and/or credentials.");
            }

            // Based on the service requesting the authentication token, lookup the associated provider.  
            IAuthProvider provider = GetProvider(authCommand);
            if (provider == null)
            {
                return AuthResult.Failed(
                    $"Requests from Api: {authCommand.Context.RequestingApi} can't be authenticated.");
            }

            // Delegate to the provider to determine if the submitted credentials are valid
            // for the service requesting the authentication token.
            if (! provider.IsValidAuthenticationRequest(authCommand.Context))
            {
                return AuthResult.Failed("Invalid Authentication Request");
            }

            // Since valid credential values have been received, delegate to the provider 
            // to preform the authentication.
            AuthResult authResult = await provider.OnAuthenticateAsync(authCommand.Context);

            authResult = SetTokenForAuthenticationRequest(authResult, provider);

            _logger.LogTraceDetails(LogEvents.AuthResultDetermined, 
                "Authentication results determined by provider.", authResult);
    
            return authResult;
        }

        private AuthResult SetTokenForAuthenticationRequest(AuthResult authResults, IAuthProvider provider)
        {
            if (! authResults.IsAuthenticated) {
                return authResults;
            }

            string token = _tokenSrv.CreateClaimToken(authResults.ResourcePermissions);
            return authResults.SetSignedToken(token);
        }

        private IAuthProvider GetProvider(AuthenticateCaller authCommand)
        {
            // Delegate to the provider module to determine the provider associated with the service-API
            // requesting authentication.
            IAuthProvider provider = _providerModule.GetServiceAuthProvider(_lifetimeScope, authCommand.Context.RequestingApi);
            if (provider == null)
            {
                _logger.LogError(LogEvents.UnknownRequestingService, 
                    "Authentication Provider for Service not be Resolved: {ServiceName}", 
                    authCommand.Context.RequestingApi);

                return null;
            }

            _logger.LogDebug(LogEvents.AuthProviderResolved, 
                "Authentication for {ServiceName} being handled by Provider: {ProviderType}", 
                authCommand.Context.RequestingApi, provider.GetType().FullName);

            return provider;
        }
    }
}
