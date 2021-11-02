using AngleSharp.Parser;
using Inshaker.Client;
using InshakerParser.Data;
using InshakerParser.Data.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InshakerParser
{
    public class DetailsWorker : BackgroundService
    {
        private readonly MongoConnection _mongoConnection;
        private readonly AngleSharpParser _angleSharpParser;
        private readonly InshakerClient _inshakerClient;
        private readonly ILogger<DetailsWorker> _logger;
        private Timer _processTimer;
        public DetailsWorker(
            MongoConnection mongoConnection,
            AngleSharpParser angleSharpParser,
            InshakerClient inshakerClient,
            ILogger<DetailsWorker> logger)
        {
            _mongoConnection = mongoConnection;
            _angleSharpParser = angleSharpParser;
            _inshakerClient = inshakerClient;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var batchSize = 3;
            _processTimer = new Timer(async (a) => await ProcessBatchAsync(batchSize, stoppingToken),
                state: null,
                dueTime: 0,
                period: Timeout.Infinite);

            return Task.CompletedTask;
        }

        private async Task ProcessBatchAsync(int batchSize, CancellationToken cancellationToken)
        {
            var batch = _mongoConnection.GetQuery<Cocktail>().Where(x => !x.Processed).OrderBy(x => x.Id).Take(batchSize)
                .ToList();
            if (!batch.Any())
            {
                // try again after 3 seconds
                _processTimer.Change(dueTime: 3000, period: Timeout.Infinite);
                return;
            }

            var tasks = new List<Task>();
            foreach (var item in batch)
            {
                tasks.Add(Task.Run(async () => await FillDetailsAsync(item, cancellationToken)));
            }
            await Task.WhenAll(tasks);
            // manually start timer again 
            _processTimer.Change(dueTime: 0, period: Timeout.Infinite);
        }

        private async Task FillDetailsAsync(Cocktail cocktail, CancellationToken ct)
        {
            try
            {
                var detailsHtml = await _inshakerClient.GetDetailsAsync(cocktail.RelativeDetailsUrl, ct);

                var detailsDto = await _angleSharpParser.ParseCocktailDetails(detailsHtml, ct);


                cocktail.Ingredients = detailsDto.Ingredients.Select(x => new Ingredient()
                {
                    Amount = x.Amount,
                    Title = x.Title,
                    Units = x.Unit
                }).ToList();

                cocktail.Stuffs = detailsDto.Stuffs.Select(x => new Stuff()
                {
                    Amount = x.Amount,
                    Title = x.Title,
                    Units = x.Unit
                }).ToList();

                var update = Builders<Cocktail>.Update
                    .Set(x => x.Ingredients, cocktail.Ingredients)
                    .Set(x => x.Stuffs, cocktail.Stuffs)
                    .Set(x => x.Processed, true);

                await _mongoConnection.GetCollection<Cocktail>().FindOneAndUpdateAsync(x => x.Id == cocktail.Id, update);

                _logger.LogInformation("Successfully processed details for cocktail {0} id={1}", cocktail.Title, cocktail.Id);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "An error occured while processing cocktail id={0}", cocktail.Id);
            }
        }
    }
}
