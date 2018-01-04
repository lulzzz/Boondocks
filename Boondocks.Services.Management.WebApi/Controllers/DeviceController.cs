using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boondocks.Services.Management.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/device")]
    public class DeviceController : Controller
    {
        [HttpGet]
        public Task<GetDevicesResponse> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public Task<GetDevicesResponse> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<CreateDeviceResponse> Post()
        {
            throw new NotImplementedException();
        }
    }
}