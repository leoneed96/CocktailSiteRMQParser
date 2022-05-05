using AngleSharp.Html.Parser;
using AngleSharp.Parser.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AngleSharp.Parser
{
    public class AngleSharpParser
    {
        private readonly ILogger<AngleSharpParser> _logger;

        public AngleSharpParser(ILogger<AngleSharpParser> logger)
        {
            _logger = logger;
        }

        public async Task<CocktailDetails> ParseCocktailDetails(string html, CancellationToken ct)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(html);
            var result = new CocktailDetails();

            var tablesContainer = document.QuerySelector(".ingredient-tables");
            //ingredients
            var ingredientTable = tablesContainer.QuerySelectorAll("table")[0];
            var ingredientRows = ingredientTable.QuerySelectorAll("td.name");
            foreach (var item in ingredientRows)
            {
                var amount = item.NextElementSibling;
                var units = amount.NextElementSibling;

                var ing = new Ingredient()
                {
                    Title = item.QuerySelector("a").TextContent,
                    Amount = double.Parse(amount.TextContent),
                    Unit = units.TextContent
                };
                result.Ingredients.Add(ing);
            }
            // stuff
            var stuffTable = tablesContainer.QuerySelectorAll("table")[1];
            var stuffRows = stuffTable.QuerySelectorAll("td.name");
            foreach (var item in stuffRows)
            {
                var amount = item.NextElementSibling;
                var units = amount.NextElementSibling;

                var stuff = new Stuff()
                {
                    Title = item.QuerySelector("a").TextContent,
                    Amount = double.Parse(amount.TextContent),
                    Unit = units.TextContent
                };
                result.Stuffs.Add(stuff);
            }

            // receipt
            var receiptContainer = document.QuerySelector(".recipe");
            var receiptList = receiptContainer.QuerySelector("ul").QuerySelectorAll("li");

            foreach (var item in receiptList)
                result.Receipt.Add(item.TextContent);

            //about
            var aboutContainer = document.QuerySelector("#cocktail-tag-text");
            var about = aboutContainer?.QuerySelector("blockquote.body")?.QuerySelector("p");
            result.About = about?.TextContent ?? aboutContainer?.TextContent;
            //imageUrl
            var imageDiv = document.QuerySelector(".common-image-frame");
            var relativeLink = imageDiv?.GetAttribute("lazy-bg");
            result.RelativeImageUrl = relativeLink;

            return result;
        }
    }
}
