using System;
using Boondocks.Services.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    public class DeviceAuthenticationOptions : AuthenticationSchemeOptions
    {
        //TODO: Add stuff to talk to the database here
        public DeviceAuthenticationOptions()
        {
            
        }

        public IDbConnectionFactory ConnectionFactory { get; set; }        
    }

}