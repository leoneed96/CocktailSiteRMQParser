using AngleSharp.Html.Parser;
using AngleSharp.Parser.Models;
using Microsoft.Extensions.Logging;
using System;
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

        public async Task<List<CocktailListItem>> ParseCocktailsPage(string html, CancellationToken ct)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(html);
            var coctailItems = document.QuerySelectorAll(".cocktail-item");

            var result = new List<CocktailListItem>();
            foreach (var item in coctailItems)
            {
                ct.ThrowIfCancellationRequested();

                var entry = new CocktailListItem();

                entry.Id = item.GetAttribute("data-id");
                entry.Link = item.QuerySelector("a.cocktail-item-preview").GetAttribute("href");
                entry.Title = item.QuerySelector(".cocktail-item-name").TextContent;

                _logger.LogInformation("Parsed cocktail list page item: Id:{0}, Link:{1}, Title:{2}",
                    entry.Id, entry.Link, entry.Title);
                result.Add(entry);
            }

            return result;
        }

        public async Task<CocktailDetails> ParseCocktailDetails(string html, CancellationToken ct)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(html);
            var result = new CocktailDetails();
            var tablesContainer = document.QuerySelector(".ingredient-tables");
            var ingredientTable = tablesContainer.QuerySelectorAll("table")[0];
            var stuffTable = tablesContainer.QuerySelectorAll("table")[1];

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
            return result;
        }
    }
}
