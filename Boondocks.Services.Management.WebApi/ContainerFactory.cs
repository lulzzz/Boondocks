using Autofac;
using Microsoft.AspNetCore.Hosting.Internal;

namespace Boondocks.Services.Management.WebApi
{
    public static class ContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();


            return builder.Build();
        }
    }
}