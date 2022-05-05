using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Oxford.Client
{
    public abstract class WebClientBase
    {
        protected readonly IHttpClientFactory HttpClientFactory;

        protected readonly ILogger<OxfordClient> Logger;
        protected WebClientBase(IHttpClientFactory httpClientFactory, ILogger<OxfordClient> logger)
        {
            HttpClientFactory = httpClientFactory;
            Logger = logger;
        }

        public abstract string BaseUrl { get; }
    }
}
