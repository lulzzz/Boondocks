using Autofac;
using Boondocks.Auth.App;
using Boondocks.Auth.App.Modules;
using Boondocks.Auth.Infra.Configs;
using Boondocks.Auth.Infra.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetFusion.Bootstrap.Exceptions;
using NetFusion.Bootstrap.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Boondocks.Auth.Infra.Modules
{
    /// <summary>
    /// Plugin module that is invoked during the application bootstrap process.  
    /// This module will load the certificate and cache the security key used for 
    /// signing the authentication token.  If the security key can't be created 
    /// using the certificate, an exception is thrown preventing the microservice 
    /// from starting.
    /// </summary>
    public class AuthCertificateModule : PluginModule,
        IAuthCertificateModule
    {
        private ILogger _logger;
        private JwtTokenSettings _tokenSettings;
        private X509SecurityKey _privateKey;

        // Private key used to sign the authentication token.
        public X509SecurityKey GetPrivateKey() => _privateKey;

        public override void StartModule(IContainer container, ILifetimeScope scope)
        {
            _logger = Context.Logger;

            // Load the settings.  NetFusion will throw an exception if the settings
            // do not pass validation.
            _tokenSettings = scope.Resolve<JwtTokenSettings>();

            // Load the x509 certificate from file.
            X509Certificate2 cert = LoadCertificate(_tokenSettings);

            // Store reference to the certificate private-key.
            _privateKey = new X509SecurityKey(cert) {
                KeyId = JwtKid.GetHeaderValueForCert(cert)
            };
        }

        private X509Certificate2 LoadCertificate(JwtTokenSettings tokenSettings)
        {
            byte[] certBytes = ReadCertificateBytes(tokenSettings);
            X509Certificate2 cert = null;
            
            try {
                cert = new X509Certificate2(certBytes);
            }
            catch(Exception ex)
            {
                throw new ContainerException(
                    $"Certificate could not be loaded from file: {tokenSettings.CertificateFilePath}",
                    ex);
            }

            _logger.LogDebug(LogEvents.CertFileLoaded, "Certificate file loaded from: {CertFilePath}", 
                tokenSettings.CertificateFilePath);

            return cert;
        }

        // Reads certificate containing Public/Private keys
        private byte[] ReadCertificateBytes(JwtTokenSettings tokenSettings)
        {
            if (! File.Exists(tokenSettings.CertificateFilePath))
            {
                throw new FileNotFoundException("Certificate file could not found.", 
                tokenSettings.CertificateFilePath);
            }

            byte[] fileBytes = null;
            try {
                fileBytes = File.ReadAllBytes(tokenSettings.CertificateFilePath);
            }
            catch(Exception ex)
            {
                throw new FileLoadException("Certificate file could not be loaded.", 
                    tokenSettings.CertificateFilePath, ex);
            }

            return fileBytes;
        }

        // Add to the application-container composite log.
        public override void Log(IDictionary<string, object> moduleLog)
        {
            moduleLog["Auth:Cert:Settings"] = new Dictionary<string, object>
            {
                { "Issuer", _tokenSettings.Issuer },
                { "Audience", _tokenSettings.Audience },
                { "ValidForMinutes", _tokenSettings.ValidForMinutes },
                { "CertificateFilePath", _tokenSettings.CertificateFilePath }
            };
        }
    }
}