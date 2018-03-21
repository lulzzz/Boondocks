using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Repositories;
using Boondocks.Auth.Infra;
using Boondocks.Auth.Infra.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Boondocks.Auth.Tests
{
    /// <summary>
    /// Unit tests for the provider that authenticates using active-directory.
    /// </summary>
    public class ActiveDirectoryAuthProviderUnitTest
    {
        /// <summary>
        /// This provider requires the user-name and password to be present in the credentials.
        /// </summary>
        [Fact(DisplayName = nameof(ContextMissing_UsernameAndPassword_Invalid))]
        public void ContextMissing_UsernameAndPassword_Invalid()
        {
            // Arrange:
            var authContext = AuthContext.FromService(
                "mock-service-api",
                new Dictionary<string, string> { { "unexpected-cred", "mock-value" } });

            var providerUnderTest = SetupActiveDirectoryAuthProvider();

            // Act:
            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

            // Assert:
            Assert.False(result);
        }

        /// <summary>
        /// This provider is valid if the credentials contain the user-name an password.
        /// </summary>
        [Fact(DisplayName = nameof(ContextMustContain_UsernameAndPassword_Valid))]
        public void ContextMustContain_UsernameAndPassword_Valid()
        {
            // Arrange:
            var authContext = AuthContext.FromService(
                "mock-service-api",
                new Dictionary<string, string> {
                    { "username", "mock-value" },
                    { "password", "mock-value2" }
                });

            var providerUnderTest = SetupActiveDirectoryAuthProvider();

            // Act:
            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

            // Assert:
            Assert.True(result);
        }

        /// <summary>
        /// This provider is invalid if it contains extra credentials outside of the 
        /// user-name and password.
        /// </summary>
        [Fact(DisplayName = nameof(ContextMustContain_UsernameAndPasswordExactly))]
        public void ContextMustContain_UsernameAndPasswordExactly()
        {
            // Arrange:
            var authContext = AuthContext.FromService(
                "mock-service-api",
                new Dictionary<string, string> {
                    { "unexpected-cred", "mock-value" },
                    { "username", "mock-value" },
                    { "password", "mock-value2" }
                });

            var providerUnderTest = SetupActiveDirectoryAuthProvider();

            // Act:
            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

            // Assert:
            Assert.False(result);
        }

        private ActiveDirectoryAuthProvider SetupActiveDirectoryAuthProvider(ILdapAuthRepository ldapRepo = null)
        {
            var mockLogger = new Mock<ILogger<ActiveDirectoryAuthProvider>>();
            var mockRepo = ldapRepo ?? new Mock<ILdapAuthRepository>().Object;

            return new ActiveDirectoryAuthProvider(mockLogger.Object, new LdapSettings{ }, ldapRepo);
        }
    }
}
