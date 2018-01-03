using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Common
{
    public abstract class DeviceControllerBase : Controller
    {
        private readonly Lazy<AuthorizationResult> _authentication;

        protected DeviceControllerBase()
        {
            _authentication = new Lazy<AuthorizationResult>(GetAuthentication);
        }

        private AuthorizationResult GetAuthentication()
        {
            const string Basic = "Basic ";

            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith(Basic))
            {
                return null;
            }

            string encodedUsernamePassword = authorization.Substring(Basic.Length);

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            //TODO: Verify the devicekey / password

            return new AuthorizationResult()
            {
                DeviceKey = username
            };
        }

        protected AuthorizationResult Authorization => _authentication.Value;
    }

    public class AuthorizationResult
    {
        public string DeviceKey { get; set; }
    }
}