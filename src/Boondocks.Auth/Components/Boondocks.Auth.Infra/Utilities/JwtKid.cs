using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Boondocks.Auth.Infra.Utilities
{
    public class JwtKid 
    {
        private const int RAW_KID_LENGTH = 48;       

        public static string GetHeaderValueForCert(X509Certificate2 certificate) 
        {
            X509Certificate bouncyCert = DotNetUtilities.FromX509Certificate(certificate);
            AsymmetricKeyParameter publicKey = bouncyCert.GetPublicKey();
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);

            byte[] encodedDerValue = publicKeyInfo.GetDerEncoded();
            byte[] hash = null;

            using (SHA256Managed sha256 = new SHA256Managed())
            {
                hash = sha256.ComputeHash(encodedDerValue);
            }

            byte[] subValue = hash.Take(30).ToArray();
            string base32EncodedSub = Base32.Encode(subValue);

            return FormatKid(base32EncodedSub);
        }

        private static string FormatKid(string rawValue)
        {
            if (rawValue.Length != RAW_KID_LENGTH) {
                throw new InvalidOperationException(
                    $"Invalid Raw kid value length of: {rawValue.Length} does not match "  +
                    $"expected length of: {RAW_KID_LENGTH} ");
            }

            var parts = new string[12];
            int startPos = 0;

            for (int i = 0; i < parts.Length; i++) 
            {
                parts[i] = rawValue.Substring(startPos, 4);
                startPos += 4;
            }
            return string.Join(":", parts);
        }
    }
}

