using Boondocks.Services.Device.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/DeviceConfiguration")]
    [Authorize]
    public class DeviceConfigurationController : ControllerBase
    {
        [HttpGet]
        public GetDeviceConfigurationResponse Get()
        {
            return new GetDeviceConfigurationResponse()
            {
                EnvironmentVariables = new EnvironmentVariable[]
                {
                    new EnvironmentVariable()
                    {
                        Name = "variable1",
                        Value = "value1"
                    }, 
                }
            };
        }
    }
}