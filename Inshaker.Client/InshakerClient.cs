using Inshaker.Client.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshaker.Client
{
    public class InshakerClient: WebClientBase
    {
        public override string BaseUrl => "https://ru.inshaker.com/";
        public InshakerClient(IHttpClientFactory httpClientFactory, 
            ILogger<InshakerClient> logger): base(httpClientFactory, logger)
        {
        }

        /// <summary>
        /// Gets html markup for page with coctails list. Order is random
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPageAsync(int page, CancellationToken ct)
        {
            using var client = HttpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(GetPageUrl(page), ct);
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length == 0)
                    throw new EmptyCocktailListPageException();
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
                Logger.LogError("An unexpected error occurred while requesting page {0}. {1}", page, e.ToString());
                throw;
            }
        }

        public async Task<string> GetDetailsAsync(string relativeDetailsLink, CancellationToken ct)
        {
            using var client = HttpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(GetDetailsUrl(relativeDetailsLink), ct);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {

                    Logger.LogError("Http request for page finished with bad status code. Code={0}, Response={1}",
                        response.StatusCode, content);

                    throw new Exception();
                }
                return content;
            }
            catch (Exception e)
            {
                Logger.LogError("An unexpected error occurred while getting details by link {0}. {1}", relativeDetailsLink, e.ToString());
                throw;
            }
        }

        private string GetPageUrl(int page) => $"{BaseUrl}cocktails?random_page={page}&pagination=random";
        private string GetDetailsUrl(string relativeLink) => $"{BaseUrl}{relativeLink}";

    }
}
