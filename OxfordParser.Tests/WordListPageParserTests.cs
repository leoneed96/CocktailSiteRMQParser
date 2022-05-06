using AngleSharp.Html.Parser;
using AngleSharp.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Oxford.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordParser.Tests
{
    [TestClass]
    public class WordListPageParserTests
    {
        private readonly WordListPageParser wordListPageParser = new WordListPageParser();
        private readonly WordDetailsParser wordDetailsParser = new WordDetailsParser();
        private readonly OxfordClient client;

        public WordListPageParserTests()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddLogging();

            var provider = services.BuildServiceProvider();

            client = new OxfordClient(provider.GetRequiredService<IHttpClientFactory>(), Mocks.GetLogger<OxfordClient>());
        }
        [TestMethod]
        public void AllModelFieldsMustBeFilled()
        {
            var html = File.ReadAllText("3000-5000.html");
            var enumerable = wordListPageParser.GetWordListItems(html);
            var totalItems = 3;
            var current = 0;
            foreach (var item in enumerable)
            {
                if (current == totalItems)
                    return;
                Assert.IsNotNull(item);
                Assert.IsNotNull(item.DetailsLink);
                Assert.IsNotNull(item.UKSoundLink);
                Assert.IsNotNull(item.AmericanSoundLink);
                Assert.IsNotNull(item.WordText);
                Assert.IsNotNull(item.Type);
                current++;
            }
        }

        [TestMethod]
        public async Task TestDetails()
        {
            var outputFileName = "test_details.json";
            var results = new StringBuilder();

            var html = File.ReadAllText("3000-5000.html");
            var enumerable = wordListPageParser.GetWordListItems(html);
            foreach (var item in enumerable.Skip(100).Take(50))
            {
                Assert.IsNotNull(item);
                Assert.IsNotNull(item.DetailsLink);
                Assert.IsNotNull(item.UKSoundLink);
                Assert.IsNotNull(item.AmericanSoundLink);
                Assert.IsNotNull(item.WordText);
                Assert.IsNotNull(item.Type);

                var detailsHtml = await client.GetDetailsAsync(item.DetailsLink, CancellationToken.None);
                var details = await wordDetailsParser.GetWordDetails(detailsHtml);
                Assert.IsNotNull(details);

                results.Append(JsonConvert.SerializeObject(details));
                results.AppendLine();
            }

            File.WriteAllText(outputFileName, results.ToString());

        }


        [TestMethod]
        public void TestHtmlParser()
        {
            var parser = new HtmlParser();
            var d = parser.ParseDocument("<span class=\"x\">She cycled round the corner, <span class=\"cl\">lost her balance</span> and fell off.</span>");
            var notExisted = d.QuerySelector(".asdyuibjk");
            
        }
    }
}
