﻿using System;
using System.Linq;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.Management.WebApi.Model;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using DataAccess.Domain;

    [Produces("application/json")]
    [Route("v1/deviceTypes")]
    public class DeviceTypesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceTypesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public DeviceType[] Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<DeviceType>("select Id, Name, CreatedUtc from DeviceTypes order by Name")
                    .ToArray();               
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(DeviceType))]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection.Get<DeviceType>(id)
                    .ObjectOrNotFound();
            }
        }

        [HttpPost]
        public DeviceType Post([FromBody]CreateDeviceTypeRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                DeviceType deviceType = new DeviceType()
                {
                    Name = request.Name
                }.SetNew();

                connection.Insert(deviceType);

                return deviceType;
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody]DeviceType deviceType)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //Execute the update
                return connection
                    .Execute("update DeviceTypes set Name = @Name where Id = @Id", deviceType)
                    .HandleUpdateResult();
            }
        }

        //[HttpDelete("{id}")]
        //public IActionResult Delete(Guid id)
        //{
        //    using (var connection = _connectionFactory.CreateAndOpen())
        //    {
        //        connection.Execute("delete DeviceTypes where Id = @id", new {id})
        //            .HandleUpdateResult();
        //    }

        //    return Ok();
        //}
    }
}