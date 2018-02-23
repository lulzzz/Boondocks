namespace Boondocks.Services.WebApiClient
{
    using Microsoft.AspNetCore.WebUtilities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class ApiClient : IDisposable
    {
        private readonly Uri _baseUri;
        private readonly HttpClient _client;
        private static readonly TimeSpan s_InfiniteTimeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        private readonly IEnumerable<Action<ApiResponse>> _errorHandlers = new Action<ApiResponse>[]
        {
            r =>
            {
                if (r.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedApiException(r);
                }
            }
        };

        public ApiClient(Uri baseUri, TimeSpan? defaultTimeout = null)
        {
            _baseUri = baseUri;
            _client = new HttpClient();

            DefaultTimeout = defaultTimeout ?? s_InfiniteTimeout;

            JsonSerializer = new JsonSerializer();
        }

        protected virtual string UserAgent => "ApiClient";

        public async Task<ApiResponse<string>> MakeRequestAsync(
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            object queryString = null,
            IDictionary<string, string> headers = null,
            Func<HttpContent> content = null)
        {
            using (var response = await InternalMakeRequestAsync(DefaultTimeout,
                HttpCompletionOption.ResponseContentRead, method, path, queryString, headers, content, cancellationToken))
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var apiResponse = new ApiResponse<string>(response.StatusCode, responseBody, response.Headers);

                HandleIfErrorResponse(apiResponse);

                return apiResponse;
            }
        }

        public async Task<ApiResponse<Stream>> MakeRequestForStreamedResponseAsync(
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            object queryString = null)
        {
            var response = await InternalMakeRequestAsync(s_InfiniteTimeout, HttpCompletionOption.ResponseHeadersRead, method, path, queryString, null, null, cancellationToken);

            var body = await response.Content.ReadAsStreamAsync();

            var apiResponse = new ApiResponse<Stream>(response.StatusCode, body, response.Headers);

            HandleIfErrorResponse(apiResponse);

            return apiResponse;
        }

        private async Task<HttpResponseMessage> InternalMakeRequestAsync(
            TimeSpan timeout,
            HttpCompletionOption completionOption,
            HttpMethod method,
            string path,
            object queryString,
            IDictionary<string, string> headers,
            Func<HttpContent> content,
            CancellationToken cancellationToken)
        {
            var request = PrepareRequest(method, path, queryString, headers, content);

            if (timeout != s_InfiniteTimeout)
            {
                var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutTokenSource.CancelAfter(timeout);
                cancellationToken = timeoutTokenSource.Token;
            }

            var response = await _client.SendAsync(request, completionOption, cancellationToken);

            //TODO: Handle authentication challenge

            return response;
        }

        private void HandleIfErrorResponse(ApiResponse response)
        {
            // If no customer handlers just default the response.
            foreach (var handler in _errorHandlers)
            {
                handler(response);
            }

            // No custom handler was fired. Default the response for generic success/failures.
            if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.BadRequest)
            {
                throw new ApiException(response);
            }
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

            UriBuilder builder = new UriBuilder(_baseUri + relativePath + idPart);

            IDictionary<string, string> query = ParseQueryString(builder.Query);

            if (parameters != null)
            {
                foreach (var parameter in parameters.Where(p => p.Key != idKey))
                {
                    query[parameter.Key] = parameter.Value;
                }
            }

            builder.Query = QueryHelpers.AddQueryString(builder.Query, query);

            return builder.ToString();
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

        internal HttpRequestMessage PrepareRequest(HttpMethod method, string path, object routeValues, IDictionary<string, string> headers, Func<HttpContent> content)
        {
            if (string.IsNullOrEmpty("path"))
            {
                throw new ArgumentNullException(nameof(path));
            }

            string uri = CreateRequestUri(path, routeValues.ToDictionary());

            var request = new HttpRequestMessage(method, uri);

            request.Headers.Add("User-Agent", UserAgent);

            if (headers != null && !headers.ContainsKey("Content-Type"))
            {
                request.Headers.Add("Content-Type", "application/json");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            //Create the content
            request.Content = content?.Invoke();

            return request;
        }

        public TimeSpan DefaultTimeout { get; set; }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public JsonSerializer JsonSerializer { get; }
    }
}