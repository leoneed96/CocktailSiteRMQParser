using Inshaker.Client.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshaker.Client
{
    public class InshakerClient
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<InshakerClient> _logger;
        private const string BaseUrl = "https://ru.inshaker.com/";
        public InshakerClient(IHttpClientFactory httpClientFactory, 
            ILogger<InshakerClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Gets html markup for page with coctails list. Order is random
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPageAsync(int page, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(GetPageUrl(page), ct);
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length == 0)
                    throw new EmptyCocktailListPageException();
                if (!response.IsSuccessStatusCode)
                {

                    _logger.LogError("Http request for page finished with bad status code. Code={0}, Response={1}",
                        response.StatusCode, content);

                    throw new Exception();
                }
                return content;
            }
            catch(Exception e)
            {
                _logger.LogError("An unexpected error occurred while requesting page {0}. {1}", page, e.ToString());
                throw;
            }
        }

        public async Task<string> GetDetailsAsync(string relativeDetailsLink, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(GetDetailsUrl(relativeDetailsLink), ct);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {

                    _logger.LogError("Http request for page finished with bad status code. Code={0}, Response={1}",
                        response.StatusCode, content);

                    throw new Exception();
                }
                return content;
            }
            catch (Exception e)
            {
                _logger.LogError("An unexpected error occurred while getting details by link {0}. {1}", relativeDetailsLink, e.ToString());
                throw;
            }
        }

        private string GetPageUrl(int page) => $"{BaseUrl}cocktails?random_page={page}&pagination=random";
        private string GetDetailsUrl(string relativeLink) => $"{BaseUrl}{relativeLink}";

    }
}
