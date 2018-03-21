using Boondocks.Auth.Api.Commands;
using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Tests.Mocks;
using Boondocks.Auth.Tests.Setup;
using NetFusion.Test.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Boondocks.Auth.Tests
{
    /// <summary>
    /// Tests that the submitted information required to preform an authentication
    /// is correctly populated on the command.  These tests intercept the command
    /// being sent and validates that it was correctly populated based on the data
    /// received by the controller from the posted request.  These are integration
    /// tests that making HTTP request for which the created command is asserted.
    /// </summary>
    public class WebApiRequestIntegrationTests
    {
        /// <summary>
        /// Tests that the specified credentials posted as the message body are correctly
        /// populated on the command. 
        /// </summary>
        [Fact (DisplayName = nameof(ClientCredentialsReceived_WhenSent))]
        public async Task ClientCredentialsReceived_WhenSent()
        {
           // Arrange:
            var mockMessaging = MockMessagingService.Setup(results => {
                var expectedResult = AuthResult.Authenticated().SetSignedToken("MOCK_TOKEN");
                results.RegisterResponse<AuthenticateCaller, AuthResult>(expectedResult);
            });
            
            var plugin = new MockAppHostPlugin();
            var httpClient = TestHttpClient.Create(plugin, mockMessaging);

            // Act:
            var credentialModle = new AuthCredentialModel
            {
                Credentials = new Dictionary<string, string>
                {
                    { "CertKey1", "CertValue1" },
                    { "CertKey2", "CertValue2" }
                }
            };
     
            var result = await httpClient.AuthenticateAsync(credentialModle);

            // Assert:
            Assert.True(mockMessaging.ReceivedMessages.Count() == 1);

            var receivedCommand = (AuthenticateCaller)mockMessaging.ReceivedMessages.First();
            Assert.NotNull(receivedCommand.Context);

            var receivedCredentials = receivedCommand.Context.Credentials;
            Assert.NotNull(receivedCredentials);

            Assert.True(receivedCredentials.ContainsKey("CertKey1"));
            Assert.True(receivedCredentials.ContainsKey("CertKey2"));
            Assert.Equal("CertValue1", receivedCredentials["CertKey1"]);
            Assert.Equal("CertValue2", receivedCredentials["CertKey2"]);
        }

        /// <summary>
        /// Tests that the specified resource service and scope query string parameters are correctly 
        /// populated on the command.  The resource scope provides information about the resources
        /// owned by a service (Docker Registry).  For a Docker Registry, the scope values specify
        /// docker repository resources.
        /// </summary>
        [Fact(DisplayName = nameof(ResourcesToAuthenticateAccess_Received))]
        public async Task ResourcesToAuthenticateAccess_Received()
        {
           // Arrange:
            var mockMessaging = MockMessagingService.Setup(results => {
                var expectedResult = AuthResult.Authenticated().SetSignedToken("MOCK_TOKEN");
                results.RegisterResponse<AuthenticateCaller, AuthResult>(expectedResult);
            });

            var expectedService = "resource-owner";
            var scope1 = "repository:test/my-app:pull,push";
            var scope2 = "repository:test/my-app2:pull";

            var url = $@"api/boondocks/authentication?service={expectedService}&scope={scope1}&scope={scope2}";

            var plugin = new MockAppHostPlugin();
            var httpClient = TestHttpClient.Create(plugin, mockMessaging);

            // Act:
            var credentials = new AuthCredentialModel { };
            var result = await httpClient.AuthenticateAsync(url, credentials);

            // Assert:
            Assert.True(mockMessaging.ReceivedMessages.Count() == 1);

            var receivedCommand = (AuthenticateCaller)mockMessaging.ReceivedMessages.First();
            Assert.NotNull(receivedCommand.Context);

            // Assert Owning Service Submitted:
            Assert.Equal(receivedCommand.Context.ResourceOwner, expectedService);

            // Assert Resources to Authenticate Access:
            Assert.NotNull(receivedCommand.Context.Resources);
            Assert.True(receivedCommand.Context.Resources.Length == 2);

            // Assert First Resource Scope:
            var firstScope = receivedCommand.Context.Resources[0];
            Assert.Equal("repository", firstScope.Type);
            Assert.Equal("test/my-app", firstScope.Name);
            Assert.True(firstScope.Actions.Length == 2);
            Assert.Equal("pull", firstScope.Actions[0]);
            Assert.Equal("push", firstScope.Actions[1]);


            // Assert Second Resource Scope:
            var secondScope = receivedCommand.Context.Resources[1];
            Assert.Equal("repository", secondScope.Type);
            Assert.Equal("test/my-app2", secondScope.Name);
            Assert.True(secondScope.Actions.Length == 1);
            Assert.Equal("pull", firstScope.Actions[0]);
        }
    }
}