using System;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Contracts;
    using Dapper;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;
    using Services.Contracts;

    [Produces("application/json")]
    [Route("v1/health")]
    public class HealthController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly RegistryConfig _registryConfig;

        public HealthController(IDbConnectionFactory connectionFactory, RegistryConfig registryConfig)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _registryConfig = registryConfig ?? throw new ArgumentNullException(nameof(registryConfig));
        }

        [HttpGet]
        public async Task<GetHealthResponse> Get()
        {
            var items = new List<GetHealthResponseItem>(3)
            {
                GetDatabaseHealth(),
            };

            var secureRegistryResponse = await GetRegistryHealthAsync(true);
            var insecureRegistryResponse = await GetRegistryHealthAsync(false);

            if (secureRegistryResponse.Passed)
            {
                items.Add(secureRegistryResponse);
            }
            else if (insecureRegistryResponse.Passed)
            {
                items.Add(insecureRegistryResponse);
            }
            else
            {
                items.Add(secureRegistryResponse);
                items.Add(insecureRegistryResponse);
            }

            return new GetHealthResponse()
            {
                Items = items.ToArray()
            };
        }

        /// <summary>
        /// Wrapped up the try / catch logic for testing a health item.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private GetHealthResponseItem Test(string name, Func<string> func)
        {
            try
            {
                return new GetHealthResponseItem()
                {
                    Passed = true,
                    Name = name,
                    Message = func()
                };
            }
            catch (Exception ex)
            {
                return new GetHealthResponseItem()
                {
                    Passed = false,
                    Name = name,
                    Message = ex.Message
                };
            }
        }

        private GetHealthResponseItem GetDatabaseHealth()
        {
            return Test("database", () =>
            {
                //Talk to the database
                using (var connection = _connectionFactory.CreateAndOpen())
                {
                    //Query something so we know whether it worked or not 
                    connection.Query<DeviceType>("select * from DeviceTypes");
                }

                return "passed";
            });
        }

        private async Task<GetHealthResponseItem> GetRegistryHealthAsync(bool secured)
        {
            //Form the name
            string name = secured ? "secure registry" : "insecure registry";

            //Determine the protocol
            string protocol = secured ? "https" : "http";

            //Come up with the url for this guy
            string url = $"{protocol}://{_registryConfig.RegistryHost}/v2";

            try
            {
                using (var client = new HttpClient())
                {
                    
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        //Make the request
                        var response = await client.SendAsync(request);

                        //Format the response message
                        string message = $"Response from '{url}' was {response.StatusCode}({response.ReasonPhrase})";

                        //Now determine if it was successful
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                            case HttpStatusCode.Unauthorized:

                                return new GetHealthResponseItem
                                {
                                    Name = name,
                                    Passed = true,
                                    Message = message
                                };

                            default:

                                return new GetHealthResponseItem()
                                {
                                    Message = message,
                                    Name = name,
                                    Passed = false
                                };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new GetHealthResponseItem()
                {
                    Passed = false,
                    Name = name,
                    Message = $"{url}: {ex.Message}",
                };
            }
        }

        
    }
}