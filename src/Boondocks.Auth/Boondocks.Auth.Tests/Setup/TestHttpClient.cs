using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NetFusion.Messaging;
using NetFusion.Test.Plugins;
using System.Net.Http;

namespace Boondocks.Auth.Tests.Setup
{
    /// <summary>
    /// Creates an initialized instance of the WebHostBuilder by invoking the
    /// ASP.NET startup class and registers any additional services.
    /// </summary>
    public static class TestHttpClient
    {
        public static HttpClient Create(
            MockAppHostPlugin hostPlugin,
            IMessagingService messagingService)
        {
           
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    hostPlugin.UseMessagingPlugin();
      
                    // Creates the typical StartUp class used by ASP.NET Core.
                    var startup = new TestStartup(hostPlugin);
                    services.AddSingleton<IStartup>(startup);

                    // Adds mock messaging service so know results can be returned
                    // for sent commands.
                    services.AddSingleton(messagingService);

                }).UseSetting(WebHostDefaults.ApplicationKey, typeof(TestHttpClient).Assembly.FullName);

           
            // Create an instance of the server and create an HTTP Client 
            // to communicate with in-memory web-host.
            var server = new TestServer(builder);
            var httpClient = server.CreateClient();

            return httpClient;
        }
    }

}