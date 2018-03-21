using Boondocks.Auth.Api.Commands;
using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Tests.Mocks;
using Boondocks.Auth.Tests.Resources;
using Boondocks.Auth.Tests.Setup;
using NetFusion.Test.Plugins;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Boondocks.Auth.Test
{
    /// <summary>
    /// These tests assert that the controller returns the correct results based
    /// on the determined authentication result returned from sending a command
    /// to the application layer.  These unit-tests inject a mock MessagingService
    /// returning a known authentication result for the published command.
    /// </summary>
    public class WebApiResponseIntegrationTests
    {
        /// <summary>
        /// If the service requesting the authentication is not specified or invalid credential,
        /// parameters specified, a bad-request result is returned with a descriptive message.  
        /// </summary>
        [Fact (DisplayName = nameof(IncorrectedOrMissingCredentials_BadRequest))]
        public async Task IncorrectedOrMissingCredentials_BadRequest()
        {
            // Arrange:
            var mockMessaging = MockMessagingService.Setup(results => {

                var expectedResult = AuthResult.Failed("INVALID_CONTEXT");                
                results.RegisterResponse<AuthenticateCaller, AuthResult>(expectedResult);
            });

            var plugin = new MockAppHostPlugin();
            var httpClient = TestHttpClient.Create(plugin, mockMessaging);

            // Act:
            var credentials = new AuthCredentialModel { };
            var result = await httpClient.AuthenticateAsync(credentials);

            // Assert:
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Content);
            
            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal("INVALID_CONTEXT", content);
        }

        /// <summary>
        /// If the application service determines that the submitted credentials are specified 
        /// but fail validation, an unauthorized result is returned with descriptive message.
        /// </summary>
        [Fact (DisplayName = nameof(InvalidSpecifiedCredentials_UnAuthorized))]
        public async Task InvalidSpecifiedCredentials_UnAuthorized()
        {
            // Arrange:
            var mockMessaging = MockMessagingService.Setup(results => {
                var expectedResult = AuthResult.SetAuthenticated(false);
                results.RegisterResponse<AuthenticateCaller, AuthResult>(expectedResult);
            });

            var plugin = new MockAppHostPlugin();
            var httpClient = TestHttpClient.Create(plugin, mockMessaging);

            // Act:
            var credentials = new AuthCredentialModel { };
            var result = await httpClient.AuthenticateAsync(credentials);

            // Assert:
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }


        // <summary>
        /// If the required and valid credential values are submitted, a JWT authentication
        /// returned containing the signed token. 
        [Fact]
        public async Task ValidCredentials_OkStatus_WithTokenHeader()
        {
            // Arrange:
            var mockMessaging = MockMessagingService.Setup(results => {
                var expectedResult = AuthResult.Authenticated()
                    .SetSignedToken("MOCK_TOKEN");
                results.RegisterResponse<AuthenticateCaller, AuthResult>(expectedResult);
            });
            
            var plugin = new MockAppHostPlugin();
            var httpClient = TestHttpClient.Create(plugin, mockMessaging);

            // Act:
            var credentials = new AuthCredentialModel { };
            var result = await httpClient.AuthenticateAsync(credentials);

            // Assert:
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            result.Headers.TryGetValues("X-Custom-Token", out IEnumerable<string> values);
            Assert.True(values.Count() == 1);
            Assert.Equal("MOCK_TOKEN", values.First());
        }

        // <summary>
        /// If the required and valid credential values are submitted, the resource
        /// access information is returned in the body.
        [Fact (DisplayName = nameof(ValidCredentials_ResourceAccess_Returned))]
        public async Task ValidCredentials_ResourceAccess_Returned()
        {
            // Arrange:
            var allowedResourceAccess = new ResourcePermission[] {
                new ResourcePermission(type: "ResourceType", name: "ResourceName", actions: new string[] { "action1", "action2" })
            };

            var mockMessaging = MockMessagingService.Setup(results => {
                var expectedResult = AuthResult.Authenticated(allowedResourceAccess)
                    .SetSignedToken("MOCK_TOKEN");
                results.RegisterResponse<AuthenticateCaller, AuthResult>(expectedResult);
            });

            var plugin = new MockAppHostPlugin();
            var httpClient = TestHttpClient.Create(plugin, mockMessaging);

            // Act:
            var credentials = new AuthCredentialModel { };
            var result = await httpClient.AuthenticateAsync(credentials);

            // Assert:
            var responseValue = await result.Content.ReadAsStringAsync();
            var resource = JsonConvert.DeserializeObject<AuthResultResource>(responseValue);
            var resourcesGrantedAccess = resource.GetEmbeddedCollection<AuthAccessResource>("resource-access");

            Assert.NotNull(resourcesGrantedAccess);
            Assert.True(resourcesGrantedAccess.Count() == 1);

            var access = resourcesGrantedAccess.First();
            Assert.Equal("ResourceType", access.Type);
            Assert.Equal("ResourceName", access.Name);
            Assert.True(access.Actions.Length == 2);
            Assert.Equal("action1", access.Actions[0]);
            Assert.Equal("action2", access.Actions[1]);
        }   
    }
}