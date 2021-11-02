using AngleSharp.Parser;
using Inshaker.Client;
using Inshaker.Client.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InshakerParser.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task FirstCoctailPageReturnsNotEmptyString()
        {
            var client = new InshakerClient(Mocks.GetHttpClientFactory(), Mocks.GetLogger<InshakerClient>());
            var result = await client.GetPageAsync(0, CancellationToken.None);

            Assert.AreEqual(true, result.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyCocktailListPageException))]
        public async Task ClientThrowsExceptionWhenPageIsEmpty()
        {
            var client = new InshakerClient(Mocks.GetHttpClientFactory(), Mocks.GetLogger<InshakerClient>());
            var result = await client.GetPageAsync(1000, CancellationToken.None);
        }

        [TestMethod]
        public async Task FirstCoctailPageParsingReturnsTwentyItems()
        {
            var client = new InshakerClient(Mocks.GetHttpClientFactory(), Mocks.GetLogger<InshakerClient>());
            var result = await client.GetPageAsync(0, CancellationToken.None);
            var parser = new AngleSharpParser(Mocks.GetLogger<AngleSharpParser>());
            var parseResult = await parser.ParseCocktailsPage(result, CancellationToken.None);
            Assert.AreEqual(20, parseResult.Count);
        }

        [TestMethod]
        public async Task ParseOfFirstItemInPageShouldProcessSuccessfully()
        {
            var client = new InshakerClient(Mocks.GetHttpClientFactory(), Mocks.GetLogger<InshakerClient>());
            var result = await client.GetPageAsync(0, CancellationToken.None);
            var parser = new AngleSharpParser(Mocks.GetLogger<AngleSharpParser>());
            var parseResult = await parser.ParseCocktailsPage(result, CancellationToken.None);
            var firstItem = parseResult.First();
            var detailsHtml = await client.GetDetailsAsync(firstItem.Link, CancellationToken.None);
            var details = await parser.ParseCocktailDetails(detailsHtml, CancellationToken.None);

            Assert.AreEqual(true, details.Ingredients.Count > 0);
            Assert.AreEqual(true, details.Stuffs.Count > 0);
        }
    }
}
