using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OxfordParser.Tests  
{
    public class Mocks
    {
        public static IHttpClientFactory GetHttpClientFactory()
        {
            var factory = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient();
            factory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return factory.Object;
        }

        public static ILogger<T> GetLogger<T>()
        {
            var factory = new Mock<ILogger<T>>();
            return factory.Object;
        }
    }
}
