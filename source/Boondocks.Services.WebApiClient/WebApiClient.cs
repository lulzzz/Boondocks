using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if NETSTANDARD2_0
using Microsoft.AspNetCore.WebUtilities;
#endif

#if NET46
using System.Web;
#endif


namespace Boondocks.Services.WebApiClient
{
    public abstract class WebApiClient
    {
        /// <summary>
        /// Should end with "/"
        /// </summary>
        protected abstract string BaseUri { get; }

        /// <summary>
        /// Serializes the item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual string Serialize(object entity)
        {
            return JsonConvert.SerializeObject(entity, Formatting.Indented);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="raw"></param>
        /// <returns></returns>
        protected virtual T Deserialize<T>(string raw)
        {
            return JsonConvert.DeserializeObject<T>(raw);
        }

        protected virtual Task<HttpClient> CreateHttpClientAsync()
        {
            return Task.FromResult(new HttpClient());
        }

        protected async Task DeleteAsync(string relativePath, object routeValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            IDictionary<string, string> dictionary = routeValues.ToDictionary();

            var uri = CreateRequestUri(relativePath, dictionary);

            using (var client = await CreateHttpClientAsync())
            {
                var response = await client.DeleteAsync(uri, cancellationToken);

                CheckResponseForError(uri, response);
            }
        }

        protected async Task<TResponse> GetAsync<TResponse>(string relativePath, object routeValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var uri = CreateRequestUri(relativePath, routeValues);

            using (var client = await CreateHttpClientAsync())
            {
                var result = await client.GetAsync(uri, cancellationToken);

                return await ParseResponse<TResponse>(uri, result);
            }
        }

        protected async Task<TResponse> PutAsync<TResponse>(string relativePath, object request, object routeValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string uri = CreateRequestUri(relativePath, routeValues);

            using (var client = await CreateHttpClientAsync())
            {
                var serializedRequest = Serialize(request);

                var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

                var result = await client.PutAsync(uri, content, cancellationToken);

                return await ParseResponse<TResponse>(uri, result);
            }
        }

        protected async Task PutAsync(string relativePath, object request, object routeValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string uri = CreateRequestUri(relativePath, routeValues);

            using (var client = await CreateHttpClientAsync())
            {
                var serializedRequest = Serialize(request);

                var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

                var result = await client.PutAsync(uri, content, cancellationToken);

                CheckResponseForError(uri, result);
            }
          
        }

        /// <summary>
        /// Posts an object as json
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativePath"></param>
        /// <param name="request"></param>
        /// <param name="routeValues"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<TResponse> PostAsync<TResponse>(string relativePath, object request, object routeValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var uri = CreateRequestUri(relativePath, routeValues);

            using (var client = await CreateHttpClientAsync())
            {
                var serializedRequest = Serialize(request);

                var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(uri, content, cancellationToken);

                return await ParseResponse<TResponse>(uri, response);
            }
        }

        protected async Task<Stream> DownloadFileAsync(string relativePath, object routeValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // http://stackoverflow.com/a/25747249/232566

            var uri = CreateRequestUri(relativePath, routeValues);

            var client = await CreateHttpClientAsync();

            HttpResponseMessage response = await client.GetAsync(uri, cancellationToken);

            CheckResponseForError(uri, response);

            return await response.Content.ReadAsStreamAsync();
          
        }

        protected Dictionary<string, string> ParseQueryString(string requestQueryString)
        {
            Dictionary<string, string> rc = new Dictionary<string, string>();
            string[] ar1 = requestQueryString.Split(new char[] { '&', '?' });
            foreach (string row in ar1)
            {
                if (string.IsNullOrEmpty(row)) continue;
                int index = row.IndexOf('=');
                rc[Uri.UnescapeDataString(row.Substring(0, index))] = Uri.UnescapeDataString(row.Substring(index + 1)); // use Unescape only parts          
            }
            return rc;
        }

        /// <summary>
        /// Creates a uri based on the BaseUri, relative path and the routeValues (similar to how MVC helper methods do it).
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        protected string CreateRequestUri(string relativePath, object routeValues = null)
        {
            IDictionary<string, string> dictionary = routeValues == null ? new Dictionary<string, string>() : routeValues.ToDictionary();
            return CreateRequestUri(relativePath, dictionary);
        }

        protected string CreateRequestUri(string relativePath, IDictionary<string, string> parameters = null)
        {
            const string idKey = "id";

            string idPart = null;
            string id;

            if (parameters != null && parameters.TryGetValue(idKey, out id))
            {
                idPart = $"/{id}";
            }

            UriBuilder builder = new UriBuilder(BaseUri + relativePath + idPart);

#if NETSTANDARD2_0

            IDictionary<string, string> query = ParseQueryString(builder.Query);

            if (parameters != null)
            {
                foreach (var parameter in parameters.Where(p => p.Key != idKey))
                {
                    query[parameter.Key] = parameter.Value;
                }
            }

            builder.Query = QueryHelpers.AddQueryString(builder.Query, query);

#endif

#if NET46

            var query = HttpUtility.ParseQueryString(builder.Query);

            if (parameters != null)
            {
                foreach (var parameter in parameters.Where(p => p.Key != idKey))
                {
                    query[parameter.Key] = parameter.Value;
                }
            }

            builder.Query = query.ToString();

#endif

            return builder.ToString();
        }

        protected void CheckResponseForError(string requestUrl, HttpResponseMessage result)
        {
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Request to '{requestUrl}' failed with status code {result.StatusCode} ({(int)result.StatusCode})");
            }
        }

        protected async Task<TResponse> ParseResponse<TResponse>(string requestUrl, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new WebApiClientException($"Request to '{requestUrl}' failed: {response.StatusCode} - {response.ReasonPhrase}");
            }

            string rawResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(rawResponse))
                return default(TResponse);

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(rawResponse);
            }
            catch (Exception ex)
            {
                throw new WebApiClientException($"Unable to parse response: '{rawResponse}' from '{requestUrl}': {ex.Message}");
            }
        }
    }
}