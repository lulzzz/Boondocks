//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Microsoft.IdentityModel.Tokens;
//using Xunit;
//using System.Collections.Generic;
//using Boondocks.Auth.Infra;
//using Moq;
//using Microsoft.Extensions.Logging;
//using Boondocks.Auth.Domain.Repositories;
//using Boondocks.Auth.Domain.Entities;
//using System.Threading.Tasks;
//using System.Linq;
//using NetFusion.Common.Extensions;
//using Boondocks.Base.Auth.Core;
//using Boondocks.Base.Auth;

//namespace Boondocks.Auth.Tests
//{
//    /// <summary>
//    /// Unit tests that validate the proper execution required to
//    /// validate a request by device token.
//    /// </summary>
//    public class DeviceAuthProviderUnitTests
//    {
//        /// <summary>
//        /// To authenticate a device, the device token must be specified as
//        /// a correctly named credential.
//        /// </summary>
//        [Fact (DisplayName = nameof(ContextMustContain_DeviceToken))]
//        public void ContextMustContain_DeviceToken()
//        {
//            // Arrange:
//            var authContext = AuthContext.FromService(
//                "mock-service-api", 
//                new Dictionary<string, string> { { "unexpected-cred", "mock-value" }});

//            var providerUnderTest = SetupDeviceAuthProvider();

//            // Act:
//            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

//            // Assert:
//            Assert.False(result);
//        }

//        /// <summary>
//        /// To authenticate a device, the device token must be specified as
//        /// the only credential value.
//        /// </summary>
//        [Fact(DisplayName = nameof(ContextMustContain_OnlyOneCredentialValue))]
//        public void ContextMustContain_OnlyOneCredentialValue()
//        {
//            // Arrange:
//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                   { "device-token", "mock-value" },
//                    { "unexpected-cred2", "mock-value" }
//                });

//            var providerUnderTest = SetupDeviceAuthProvider();

//            // Act:
//            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

//            // Assert:
//            Assert.False(result);
//        }

//        /// <summary>
//        /// Request is valid if an only if the credentials contain the device token.
//        /// </summary>
//        [Fact(DisplayName = nameof(ContextMustContain_DeviceTokenCredential))]
//        public void ContextMustContain_DeviceTokenCredential()
//        {
//            // Arrange:
//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                    { "device-token", "mock-value" }
//                });

//            var providerUnderTest = SetupDeviceAuthProvider();

//            // Act:
//            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

//            // Assert:
//            Assert.True(result);
//        }

//        /// <summary>
//        /// The device credential token must contain a value.
//        /// </summary>
//        [Fact(DisplayName = nameof(ContextMustContain_DeviceTokenCredentialWithValue))]
//        public void ContextMustContain_DeviceTokenCredentialWithValue()
//        {
//            // Arrange:
//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                    { "device-token", "" }
//                });

//            var providerUnderTest = SetupDeviceAuthProvider();

//            // Act:
//            bool result = providerUnderTest.IsValidAuthenticationRequest(authContext);

//            // Assert:
//            Assert.False(result);
//        }

//        /// <summary>
//        /// The submitted device token must consist of a device-id claim to be considered valid.
//        /// </summary>
//        [Fact (DisplayName = nameof(DeviceTokenMustContain_DeviceId))]
//        public async Task DeviceTokenMustContain_DeviceId()
//        {
//            // Arrange:
//            var deviceId = Guid.Empty;
//            var deviceKey = Guid.Parse(" 671674D6-7D14-4DC0-94A0-B1085B878C23");

//            //      Create token with missing device id
//            var factory = new TokenFactory(deviceId, deviceKey);
//            var token = factory.CreateToken();

//            var providerUnderTest = SetupDeviceAuthProvider();

//            var authContext = AuthContext.FromService(
//                "mock-service-api", 
//                new Dictionary<string, string> {
//                    { "device-token", token }
//                });

//            // Act:
//            var authResult = await providerUnderTest.OnAuthenticateAsync(authContext);

//            // Assert:
//            Assert.NotNull(authResult);
//            Assert.True(authResult.IsInvalidCredentialContext);
//            Assert.False(authResult.IsAuthenticated);
//            Assert.Null(authResult.JwtSignedToken);
//        }

//        /// <summary>
//        /// For a device token to be authenticated, the repository must return the device-key
//        /// for the specified device-id.
//        /// </summary>
//        [Fact (DisplayName = nameof(DeviceIdMustHaveStored_DeviceKey))]
//        public async Task DeviceIdMustHaveStored_DeviceKey()
//        {
//            // Arrange:
//            var deviceId = Guid.Parse("B89A2796-C734-4586-9167-DFC0458B8172");
//            var deviceKey = Guid.Parse(" 671674D6-7D14-4DC0-94A0-B1085B878C23");

//            //      Create device token
//            var factory = new TokenFactory(deviceId, deviceKey);
//            var token = factory.CreateToken();

//            //      Return null to indicate device key not found
//            var mockRepo = new Mock<IDeviceKeyAuthRepository>();
//            mockRepo.Setup(m => m.GetDeviceKeyAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Guid?>(null));

//            var providerUnderTest = SetupDeviceAuthProvider(mockRepo.Object);

//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                    { "device-token", token }
//                });

//            // Act:
//            var authResult = await providerUnderTest.OnAuthenticateAsync(authContext);

//            // Assert:
//            Assert.NotNull(authResult);
//            Assert.True(authResult.IsInvalidCredentialContext);
//            Assert.False(authResult.IsAuthenticated);
//            Assert.Equal("Invalid credential token", authResult.Reason);
//        }

//        /// <summary>
//        /// If the token contains a device-id and the repository returns the corresponding 
//        /// device-key, the device is authenticated if the token was signed with the same device-key.
//        /// </summary>
//        [Fact (DisplayName = nameof(TokenMustBeSignedWithSame_SymetricKey))]
//        public async Task TokenMustBeSignedWithSame_SymetricKey()
//        {
//            // Arrange:
//            var deviceId = Guid.Parse("B89A2796-C734-4586-9167-DFC0458B8172");
//            var deviceKey = Guid.Parse(" 671674D6-7D14-4DC0-94A0-B1085B878C23");

//            //      Create device token
//            var factory = new TokenFactory(deviceId, deviceKey);
//            var token = factory.CreateToken();

//            //      Return null to indicate device key not found
//            var mockRepo = new Mock<IDeviceKeyAuthRepository>();
//            mockRepo.Setup(m => m.GetDeviceKeyAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Guid?>(deviceKey));

//            var providerUnderTest = SetupDeviceAuthProvider(mockRepo.Object);
//            var validTokenParams = new TokenValidationParameters();

//            validTokenParams.ValidIssuer = "boondocks-issuer";
//            validTokenParams.ValidAudience = "boondocks-api";

//            providerUnderTest.SetTokenValidationParameters(validTokenParams);

//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                    { "device-token", token }
//                });

//            // Act:
//            var authResult = await providerUnderTest.OnAuthenticateAsync(authContext);

//            // Assert:
//            Assert.NotNull(authResult);
//            Assert.False(authResult.IsInvalidCredentialContext);
//            Assert.True(authResult.IsAuthenticated);
//        }

//        /// <summary>
//        /// Token authentication will fail if matching user or audience does not match
//        /// the values contained within the signed device token.
//        /// </summary>
//        [Fact (DisplayName = nameof(IssuerAndAudience_MustMatch))]
//        public async Task IssuerAndAudience_MustMatch()
//        {
//            // Arrange:
//            var deviceId = Guid.Parse("B89A2796-C734-4586-9167-DFC0458B8172");
//            var deviceKey = Guid.Parse(" 671674D6-7D14-4DC0-94A0-B1085B878C23");

//            //      Create device token
//            var factory = new TokenFactory(deviceId, deviceKey);
//            var token = factory.CreateToken();

//            Console.WriteLine(token);

//            //      Return expected device key:
//            var mockRepo = new Mock<IDeviceKeyAuthRepository>();
//            mockRepo.Setup(m => m.GetDeviceKeyAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Guid?>(deviceKey));

//            var providerUnderTest = SetupDeviceAuthProvider(mockRepo.Object);
//            var validTokenParams = new TokenValidationParameters();

//            validTokenParams.ValidIssuer = "boondocks-issuer";
//            validTokenParams.ValidAudience = "boondocks-api-invalid";

//            providerUnderTest.SetTokenValidationParameters(validTokenParams);

//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                    { "device-token", token }
//                });

//            // Act:
//            var authResult = await providerUnderTest.OnAuthenticateAsync(authContext);

//            // Assert:
//            Assert.NotNull(authResult);
//            Assert.True(authResult.IsInvalidCredentialContext);
//            Assert.False(authResult.IsAuthenticated);
//        }

//        /// <summary>
//        /// When the device token passes authentication, the repository is called to determine
//        /// the resources to which the identity has access.  This resource access information is
//        /// returned as a claim containing serialized json.
//        /// </summary>
//        [Fact(DisplayName = nameof(ValidToken_ReturnsClaims))]
//        public async Task ValidToken_ReturnsClaims()
//        {
//            // Arrange:
//            var deviceId = Guid.Parse("B89A2796-C734-4586-9167-DFC0458B8172");
//            var deviceKey = Guid.Parse(" 671674D6-7D14-4DC0-94A0-B1085B878C23");

//            //      Create device token
//            var factory = new TokenFactory(deviceId, deviceKey);
//            var token = factory.CreateToken();

//            //      Return expected device key:
//            var mockRepo = new Mock<IDeviceAuthRepository>();
//            mockRepo.Setup(m => m.GetDeviceKeyAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Guid?>(deviceKey));

//            var resourceAccess = new ResourcePermission[]
//            {
//                new ResourcePermission(type: "resource", name: "repository", actions: new string[] { "pull" })
//            };
//            //      Return expected allowed resources:
//            mockRepo.Setup(m => m.GetDeviceAccessAsync(It.IsAny<Guid>(), It.IsAny<ResourcePermission[]>()))
//                    .Returns(Task.FromResult(resourceAccess));
              
//            var providerUnderTest = SetupDeviceAuthProvider(mockRepo.Object);
//            var validTokenParams = new TokenValidationParameters();

//            validTokenParams.ValidIssuer = "boondocks-issuer";
//            validTokenParams.ValidAudience = "boondocks-api";

//            providerUnderTest.SetTokenValidationParameters(validTokenParams);

//            var authContext = AuthContext.FromService(
//                "mock-service-api",
//                new Dictionary<string, string> {
//                    { "device-token", token }
//                });

//            // Act:
//            var authResult = await providerUnderTest.OnAuthenticateAsync(authContext);

//            // Assert:
//            Assert.NotNull(authResult);
//            Assert.False(authResult.IsInvalidCredentialContext);
//            Assert.True(authResult.IsAuthenticated);
//            Assert.NotNull(authResult.ResourcePermissions);
//            Assert.True(authResult.ResourcePermissions.Length == 1);
//            Assert.NotNull(authResult.Claims);

//            var accessClaim = authResult.Claims.SingleOrDefault(c => c.Type == "access");
//            Assert.NotNull(accessClaim);

//            Assert.Equal(accessClaim.Value, resourceAccess.ToJson());
//        }

//        private DeviceAuthProvider SetupDeviceAuthProvider(IDeviceAuthRepository deviceAuthRepo = null, IDeviceKeyAuthRepository deviceKeyAuthRepo)
//        {
//            var mockLogger = new Mock<ILogger<DeviceAuthProvider>>();
//            var mockAuthSrv = new Mock<IDeviceAuthService>();
//            var mockRepo = deviceAuthRepo ?? new Mock<IDeviceAuthRepository>().Object;

//            return new DeviceAuthProvider(mockLogger.Object, mockAuthSrv.Object, mockRepo);
//        }

//        private string CreateTestDeviceToken() {
//            var deviceId = Guid.Parse("B89A2796-C734-4586-9167-DFC0458B8172");
//            var deviceKey = Guid.Parse("671674D6-7D14-4DC0-94A0-B1085B878C23");

//            var factory = new TokenFactory(deviceId, deviceKey);
//            return factory.CreateToken();
//        }

//    }

//    internal class TokenFactory
//    {
//        private readonly ClaimsIdentity _claimsIdentity;
//        private readonly SigningCredentials _signingCredentials;
//        private readonly JwtSecurityTokenHandler _tokenHandler;

//        public TokenFactory(Guid deviceId, Guid deviceKey)
//        {
//            //Create the claims
//            var claims = new List<Claim>();

//            if (deviceId != Guid.Empty)
//            {
//                claims.Add(new Claim("device-id", deviceId.ToString("D")));
//            }

//            //Create the claims identity
//            _claimsIdentity = new ClaimsIdentity(claims);

//            //Create the signing credentials
//            _signingCredentials = new SigningCredentials(
//                new SymmetricSecurityKey(deviceKey.ToByteArray()),
//                SecurityAlgorithms.HmacSha256Signature);

//            _tokenHandler = new JwtSecurityTokenHandler();
//        }

//        internal string CreateToken()
//        {
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = _claimsIdentity,

//                Issuer = "boondocks-issuer",
//                Audience = "boondocks-api",

//                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5000)),
//                SigningCredentials = _signingCredentials
//            };

//            var securityToken = _tokenHandler.CreateToken(tokenDescriptor);

//            return _tokenHandler.WriteToken(securityToken);
//        }
//    }
//}