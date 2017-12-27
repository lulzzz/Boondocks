using System;
using System.Text;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/Heartbeat")]
    public class HeartbeatController : Controller
    {
        /// <summary>
        /// A heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(HeartbeatRequest request)
        {
            const string Basic = "Basic ";

            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith(Basic))
            {
                return Unauthorized();
            }

            string encodedUsernamePassword = authorization.Substring(Basic.Length);

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            return Ok(new HeartbeatResponse()
            {
                RootFileSystemVersion = authorization,

                EnvironmentVariables = new EnvironmentVariable[]
                {
                    new EnvironmentVariable()
                    {
                        Name = "RAW_AUTH",
                        Value = authorization
                    },
                    new EnvironmentVariable()
                    {
                        Name = "username",
                        Value = username
                    },
                    new EnvironmentVariable()
                    {
                        Name = "password",
                        Value = password
                    }, 
                }
            });
        }
    }
}