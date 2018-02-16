namespace Boondocks.Services.Device.WebApi.Authentication
{
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Authentication;

    public class DeviceAuthenticationOptions : AuthenticationSchemeOptions
    {
        //TODO: Add stuff to talk to the database here

        public IDbConnectionFactory ConnectionFactory { get; set; }
    }
}