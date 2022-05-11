using AngleSharp.Html.Parser;
using AngleSharp.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Oxford.Client;
using OxfordParser.Services;
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
        private readonly WordListPageParser wordListPageParser = new WordListPageParser(Mocks.GetLogger<WordListPageParser>());
        private readonly WordDetailsParser wordDetailsParser = new WordDetailsParser();
        private readonly OxfordClient client;
        private readonly FileStorageService _fileStorageService;
        public WordListPageParserTests()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddLogging();

            var opts = Options.Create(new FileStorageOptions()
            {
                SoundsRootPath = "D:\\leoneed\\EngWords\\Sounds",
                ImagesRootPath = "D:\\leoneed\\EngWords\\Images"
            });
            services.AddSingleton(opts);
            services.AddSingleton<FileStorageService>();

            var provider = services.BuildServiceProvider();

            _fileStorageService = provider.GetRequiredService<FileStorageService>();
            client = new OxfordClient(provider.GetRequiredService<IHttpClientFactory>(), Mocks.GetLogger<OxfordClient>());
        }
        [TestMethod]
        public void AllModelFieldsMustBeFilled()
        {
            var html = File.ReadAllText("3000-5000.html");
            var enumerable = wordListPageParser.GetWordListItems(html, CancellationToken.None);
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
            var enumerable = wordListPageParser.GetWordListItems(html, CancellationToken.None);
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
        public async Task TestSaveSound()
        {
            var html = File.ReadAllText("3000-5000.html");
            var enumerable = wordListPageParser.GetWordListItems(html, CancellationToken.None);
            foreach (var item in enumerable.Skip(100).Take(1))
            {
                Assert.IsNotNull(item);
                Assert.IsNotNull(item.DetailsLink);
                Assert.IsNotNull(item.UKSoundLink);
                Assert.IsNotNull(item.AmericanSoundLink);
                Assert.IsNotNull(item.WordText);
                Assert.IsNotNull(item.Type);

                var detailsHtml = await client.GetDetailsAsync(item.DetailsLink, CancellationToken.None);
                var details = await wordDetailsParser.GetWordDetails(detailsHtml);

                var ukSound = await client.GetSoundSteamAsync(item.UKSoundLink, CancellationToken.None);
                var usSound = await client.GetSoundSteamAsync(item.AmericanSoundLink, CancellationToken.None);
                await _fileStorageService.SaveSoundAsync(item.WordText, ukSound, VoiceType.UK);
                await _fileStorageService.SaveSoundAsync(item.WordText, usSound, VoiceType.US);
                Assert.IsNotNull(details);
            }
        }

        [TestMethod]
        public void UsageSelector_EnoughSentences()
        {
            var usages = new List<string>()
            {
                "Sentence 1",
                "Sentence 2"
            };
            var result = WordUsagesSelector.SelectUsages(usages, 2);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void UsageSelector_NotEnoughSentences()
        {
            var usages = new List<string>()
            {
                "Sentence 1",
                "Sentence 2",
                "- non sentence 1",
                "- non sentence 2",
                "asfbasjf"
            };
            var result = WordUsagesSelector.SelectUsages(usages, 3);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(usages[0], result[0]);
            Assert.AreEqual(usages[1], result[1]);
            Assert.AreEqual(usages[2], result[2]);
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
