using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDbMonitor.CrossCutting.QoS;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbMonitor.Clients.HttpApi
{
    public interface IHttpApiClient
    {
        Task Notify(string collectionName, ObjectId id, CancellationToken cancellation);
    }

    public class HttpApiClient : IHttpApiClient
    {
        private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate =
            delegate (HttpResponseMessage response)
            {
                if (response.StatusCode < HttpStatusCode.InternalServerError)
                    return response.StatusCode == HttpStatusCode.RequestTimeout;

                return true;
            };

        private static readonly Action<HttpResponseMessage> ThrowHttpRequestException = delegate (HttpResponseMessage response)
        {
            throw new HttpRequestException(response.ReasonPhrase) { Data = { [nameof(HttpStatusCode)] = response.StatusCode } };
        };

        private readonly IRetryProvider _retrier;
        private readonly HttpClient _client;
        private readonly HttpApiClientOptions _options;

        public HttpApiClient(IOptions<HttpApiClientOptions> options, HttpClient client, IRetryProvider retrier)
        {
            _options = options.Value;
            _client = client;
            _retrier = retrier;
        }

        public async Task Notify(string collectionName, ObjectId id, CancellationToken cancellation)
        {
            using var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutInSeconds * 2));
            using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellation);

            var response =
                await _retrier
                    .RetryOn<HttpRequestException, HttpResponseMessage>(
                        CheckError,
                        TransientHttpStatusCodePredicate,
                        () => SendRequest(_client, collectionName, id, _options, linkedSource.Token));

            if (!response.IsSuccessStatusCode)
                ThrowHttpRequestException(response);
        }

        private static bool CheckError(HttpRequestException x)
        {
            if (!x.Data.Contains(nameof(HttpStatusCode)))
                return false;

            var statusCode = (HttpStatusCode)x.Data[nameof(HttpStatusCode)];

            if (statusCode < HttpStatusCode.InternalServerError)
                return statusCode == HttpStatusCode.RequestTimeout;

            return false;
        }

        private static async Task<HttpResponseMessage> SendRequest(
            HttpClient client,
            string collectionName,
            ObjectId id,
            HttpApiClientOptions options,
            CancellationToken cancellation)
        {
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutInSeconds);

            var body = $"{{\"type\": {collectionName}, \"cacheKey\": \"{id}\"}}";

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
                RequestUri = options.ClearCacheWebhook
            };

            var response = await client.SendAsync(message, cancellation);

            return response;
        }
    }
}
