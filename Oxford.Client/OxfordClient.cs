using Oxford.Client.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Oxford.Client
{
    public class OxfordClient: WebClientBase
    {
        public static string BaseUrl => "https://www.oxfordlearnersdictionaries.com/";
        public OxfordClient(IHttpClientFactory httpClientFactory, 
            ILogger<OxfordClient> logger): base(httpClientFactory, logger)
        {
        }

        public async Task<string> GetDetailsAsync(string path, CancellationToken ct)
        {
            using var client = HttpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(GetFullUrl(path), ct);
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length == 0)
                    throw new EmptyResponseException();
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError("Http request for page finished with bad status code. Code={0}, Response={1}",
                        response.StatusCode, content);

                    throw new Exception();
                }
                return content;
            }
            catch(Exception e)
            {
                Logger.LogError(e, "An unexpected error occurred while requesting details");
                throw;
            }
        }

        public async Task<Stream> GetSoundSteamAsync(string path, CancellationToken ct)
        {
            using var client = HttpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(GetFullUrl(path), ct);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError("Http request for file finished with bad status code. Code={0}",
                        response.StatusCode);

                    throw new Exception();
                }
                return await response.Content.ReadAsStreamAsync(ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An unexpected error occurred while requesting details");
                throw;
            }
        }
        private static string GetFullUrl(string relativeUrl) => $"{BaseUrl}{relativeUrl}";
    }
}
