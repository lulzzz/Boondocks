using Autofac;
using Boondocks.Auth.Api.Commands;
using Boondocks.Auth.Api.Models;
using Boondocks.Auth.App.Ports;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;
using Boondocks.Auth.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Boondocks.Auth.Tests
{
    /// <summary>
    /// Unit tests that verify the property handling of a received authentication command response
    /// by mocking the underlying authentication provider to return known states.  Theses tests do
    /// not test for the correct population of the AuthenticateCaller command from a HTTP request.
    /// The proper population of the command from a HTTP request are tested by the integration-tests.
    /// </summary>
    public class AuthorizationPortUnitTests
    {
        /// <summary>
        /// When requesting an authentication token, the name of the service making the request
        /// must be specified.  This name is used to resolve the provider that should process
        /// the authentication request.
        /// </summary>
        [Fact (DisplayName = nameof(RequestingApi_MustBeSpecified))]
        public async Task RequestingApi_MustBeSpecified()
        {
            // Arrange:
            var resourceModel = new AuthResourceModel { };
            var credentialModel = new AuthCredentialModel { Api = "", Credentials = new Dictionary<string, string> {} }; 
            var command = new AuthenticateCaller(credentialModel, resourceModel);

            var port = CreatePort(p => { });

            // Act:
            var result = await port.AuthenticateForService(command);

            // Assert:
            Assert.NotNull(result);
            Assert.True(result.IsInvalidCredentialContext);
            Assert.Equal("Invalid authentication context and/or credentials.", result.Reason);
            Assert.False(result.IsAuthenticated);
        }

        /// <summary>
        /// When requesting an authentication token, the credentials values must be specified.
        /// If present, they are passed to the determined authentication provider for authentication.
        /// </summary>
        [Fact(DisplayName = nameof(Credentials_MustBeSpecified))]
        public async Task Credentials_MustBeSpecified()
        {
            // Arrange:
            var resourceModel = new AuthResourceModel { };
            var credentialModel = new AuthCredentialModel { Api = "service-api", Credentials = null };
            var command = new AuthenticateCaller(credentialModel, resourceModel);

            var port = CreatePort(p => { });

            // Act:
            var result = await port.AuthenticateForService(command);

            // Assert:
            Assert.NotNull(result);
            Assert.True(result.IsInvalidCredentialContext);
            Assert.Equal("Invalid authentication context and/or credentials.", result.Reason);
            Assert.False(result.IsAuthenticated);
        }

        /// <summary>
        /// The identity being authenticated can pass authentication if the property credentials 
        /// are provided.  However, for the caller to be granted access to Docker registry resources,
        /// the caller must specify the service owning the resource (Docker Registry) and one or
        /// more resources that are to be checked for access (Docker Repositories).
        /// </summary>
        [Fact(DisplayName = nameof(ResourceContextSet_WhenSpecified))]
        public async Task ResourceContextSet_WhenSpecified()
        {
            // Arrange:
            var resourceModel = new AuthResourceModel
            {
                Service = "resource-owner",
                Scope = new string[] { @"repository:test/my-app:pull,push", @"repository:test/my-app2:pull" } 
            };

            var credentialModel = new AuthCredentialModel { Api = "service-api", Credentials = new Dictionary<string, string>() };
            var command = new AuthenticateCaller(credentialModel, resourceModel);

            // Have the provider responded with a valid request
            var provider = new MockProvider { IsValidRequest = true };
            var port = CreatePort(provider);

            // Act:
            var result = await port.AuthenticateForService(command);

            // Assert:
            Assert.NotNull(provider.ReceivedContext);

            Assert.Equal("resource-owner", provider.ReceivedContext.ResourceOwner);
            Assert.True(provider.ReceivedContext.Resources.Length == 2);

            var firstResourceScope = provider.ReceivedContext.Resources[0];
            var secondResourceScope = provider.ReceivedContext.Resources[1];

            // Assert first resource scope:
            Assert.Equal("repository", firstResourceScope.Type);
            Assert.Equal("test/my-app", firstResourceScope.Name);
            Assert.True(firstResourceScope.Actions.Length == 2);
            Assert.Equal("pull", firstResourceScope.Actions[0]);
            Assert.Equal("push", firstResourceScope.Actions[1]);

            // Assert second resource scope:
            Assert.Equal("repository", secondResourceScope.Type);
            Assert.Equal("test/my-app2", secondResourceScope.Name);
            Assert.True(secondResourceScope.Actions.Length == 1);
            Assert.Equal("pull", secondResourceScope.Actions[0]);
        }

        /// <summary>
        /// An authentication command that passes the base validations, must also pass 
        /// the selected provider specific validations.
        /// </summary>
        [Fact(DisplayName = nameof(SpecifiedCredentials_MustPassProviderValidations))]
        public async Task SpecifiedCredentials_MustPassProviderValidations()
        {
            // Arrange:
            var resourceModel = new AuthResourceModel { };
            var credentialModel = new AuthCredentialModel { Api = "service-api",  Credentials = new Dictionary<string, string> {} }; 
            var command = new AuthenticateCaller(credentialModel, resourceModel);

            // Assume provider also indicates invalid submitted authentication request.
            var port = CreatePort(p => {
                p.IsValidRequest = false;
                p.HavingResult = c => throw new InvalidOperationException("Should be called for invalid request credentials");
            });

            // Act:IAuthProvider
            var result = await port.AuthenticateForService(command);

            // Assert:
            Assert.NotNull(result);
            Assert.True(result.IsInvalidCredentialContext);
            Assert.False(result.IsAuthenticated);
        }

        /// <summary>
        /// If the selected authentication provider determines that the submitted credentials
        /// are valid, it should return the correct status and the associated authentication
        /// token.
        /// </summary>
        [Fact (DisplayName = nameof(ProviderResultsReturned_ForValidCredentials))]
        public async Task ProviderResultsReturned_ForValidCredentials()
        {
            // Arrange:
            var resourceModel = new AuthResourceModel { };
            var credentialModel = new AuthCredentialModel { Api = "service-api", Credentials = new Dictionary<string, string> {} }; 
            var command = new AuthenticateCaller(credentialModel, resourceModel);

            // Create mock token service to return know token value
            var tokenSrv = new MockTokenService { ExpectedTokenValue = Guid.NewGuid().ToString() };

            // Assume provider also indicates valid submitted authentication request.
            var port = CreatePort(p => {
                p.IsValidRequest = true;
                p.HavingResult = c => AuthResult.Authenticated();
            }, tokenSrv);

            // Act:
            var result = await port.AuthenticateForService(command);

            // Assert:
            Assert.NotNull(result);
            Assert.False(result.IsInvalidCredentialContext);
            Assert.True(result.IsAuthenticated);
            Assert.Equal(tokenSrv.ExpectedTokenValue, result.JwtSignedToken);
        }

        private AuthenticationPort CreatePort(Action<MockProvider> setupProvider, 
            ITokenService tokenService = null)
        {
            // Supporting services that will not be unit tested.
            var mockLogger = new Mock<ILogger<AuthenticationPort>>();
            var mockLifetime = new Mock<ILifetimeScope>();
            var mockTokenSrv = new Mock<ITokenService>();

            // Mock authentication provider:
            var mockProvider = new MockProvider();
            setupProvider(mockProvider);

            tokenService = tokenService ?? new MockTokenService();

            var mockValidationSrv = new MockValidationService();
            
            var mockModule = new MockProviderModule(mockProvider);

            return new AuthenticationPort(mockLogger.Object, mockLifetime.Object, 
                mockModule, 
                mockValidationSrv,
                tokenService);
        }

        private AuthenticationPort CreatePort(MockProvider provider)
        {
            // Supporting services that will not be unit tested.
            var mockLogger = new Mock<ILogger<AuthenticationPort>>();
            var mockLifetime = new Mock<ILifetimeScope>();
            var mockTokenSrv = new Mock<ITokenService>();

            var mockValidationSrv = new MockValidationService();

            var mockModule = new MockProviderModule(provider);

            return new AuthenticationPort(mockLogger.Object, mockLifetime.Object,
                mockModule,
                mockValidationSrv,
                mockTokenSrv.Object);
        }
    }
}