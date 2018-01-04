using Boondocks.Services.Device.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/DeviceConfiguration")]
    public class DeviceConfigurationController : ControllerBase
    {
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