using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.WebApiClient;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    public class DeviceTypeOperations
    {
        private readonly NetworkClient _client;

        internal DeviceTypeOperations(NetworkClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public  Task<DeviceType> CreateDeviceTypeAsync(CreateDeviceTypeRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceType>(cancellationToken, HttpMethod.Post,
                ResourceUrls.DeviceTypes, request: request);
        }

        public  Task<DeviceType[]> GetDeviceTypesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceType[]>(cancellationToken, HttpMethod.Get, ResourceUrls.DeviceTypes);
        }
    }
}