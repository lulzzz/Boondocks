using Boondocks.Auth.App;
using Boondocks.Auth.App.Providers;
using Boondocks.Auth.Domain.Entities;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using System;
using System.Linq;
using System.Security.Claims;
using Boondocks.Auth.Infra.Providers;
using Boondocks.Auth.Domain.Repositories;
using NetFusion.Common.Extensions;
using System.Threading.Tasks;

namespace Boondocks.Auth.Infra
{
    /// <summary>
    /// Provider that will authenticate a user using LDAP by user name and password.
    /// </summary>
    public class ActiveDirectoryAuthProvider : AuthProviderBase
    {
        private const string UserNameProperty = "username";
        private const string UserPasswordProperty = "password";

        private ILogger<ActiveDirectoryAuthProvider> _logger;
        private LdapSettings _ldapSettings;
        private ILdapAuthRepository _ldapRepo;

        public ActiveDirectoryAuthProvider(
            ILogger<ActiveDirectoryAuthProvider> logger,
            LdapSettings ldapSettings,
            ILdapAuthRepository ldapAuthRepo)
        {
            _logger = logger;
            _ldapSettings = ldapSettings;
            _ldapRepo = ldapAuthRepo;
        }

        public override bool IsValidAuthenticationRequest(AuthContext context)
        {
            var credentials = context.Credentials;

            if (credentials.Keys.Count != 2)
            {
                 _logger.LogError(LogEvents.RequiredCredentialsError, "Invalid number of credentials received.");
                return false;
            }

            if (credentials.TryGetValue(UserNameProperty, out string userName) 
                && String.IsNullOrWhiteSpace(userName))
            {
                _logger.LogError(LogEvents.RequiredCredentialsError, 
                    $"Credentials does not contain valid property named: {UserNameProperty}");

                return false;
            }

            if (credentials.TryGetValue(UserPasswordProperty, out string userPassword) 
                && String.IsNullOrWhiteSpace(userPassword))
            {
                _logger.LogError(LogEvents.RequiredCredentialsError, 
                    $"Credentials does not contain valid property named: {UserPasswordProperty}");

                return false;
            }

            return true;
        }

        public override async Task<AuthResult> OnAuthenticateAsync(AuthContext context)
        {
            string username = context.Credentials[UserNameProperty];
            string password = context.Credentials[UserPasswordProperty];

            var authResult = await ValidateUserIdentity(username, password);
            if (authResult.IsAuthenticated)
            {
                return await GetResourceAccessClaimAsync(context, username);
            }

            return authResult;
        }

        private async Task<AuthResult> ValidateUserIdentity(string username, string password)
        {           
            bool allowedAccess = await _ldapRepo.UserAllowedAccess(username);
            if (! allowedAccess)
            {
                return AuthResult.Failed("User access not permitted.");
            }

            using (var conn = new LdapConnection())
            {              
                try{
                    conn.Connect(_ldapSettings.Connection, _ldapSettings.Port);
                    conn.Bind(AddDomain(username), password);
                    return AuthResult.Authenticated();
                }
                catch(LdapException ex)
                {
                    if (ex.ResultCode == 49)
                    {
                        return AuthResult.Failed("Credentials failed authentication");
                    }

                    _logger.LogError(LogEvents.UnexpectedAuthError, ex, "Unexpected LDAP exception.");
                    return AuthResult.Failed("Credentials failed authentication");

                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.UnexpectedAuthError, ex, "Unexpected LDAP exception.");
                    return AuthResult.Failed("Credentials failed authentication");
                }
            }
        }

        private string AddDomain(string userName)
        {
            if (! string.IsNullOrEmpty(_ldapSettings.Domain) 
                && userName.Count(c => c == '\\') == 0)
            {
                return $@"{_ldapSettings.Domain}\{userName}";
            }
            return userName;
        }

        private async Task<AuthResult> GetResourceAccessClaimAsync(AuthContext context, string userName)
        { 
            ResourcePermission[] grantedAccessToResources = await _ldapRepo.GetUserAccessAsync(userName, context.Resources);
            var accessClaim = new Claim("access", grantedAccessToResources.ToJson());

            return AuthResult.Authenticated(grantedAccessToResources, accessClaim);            
        }
    }
}
