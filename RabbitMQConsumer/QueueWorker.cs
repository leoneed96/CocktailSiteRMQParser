using AngleSharp.Parser;
using Inshaker.Client;
using InshakerParser.Data;
using InshakerParser.Data.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQTools.WorkingQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQConsumer
{
    public class QueueWorker : BackgroundService
    {
        private readonly WorkingQueueWrapper _workingQueue;
        private readonly MongoConnection _mongoConnection;
        private readonly AngleSharpParser _angleSharpParser;
        private readonly InshakerClient _inshakerClient;
        private readonly ILogger<QueueWorker> _logger;

        public QueueWorker(
            WorkingQueueWrapper workingQueue,
            MongoConnection mongoConnection,
            AngleSharpParser angleSharpParser,
            InshakerClient inshakerClient,
            ILogger<QueueWorker> logger)
        {
            _workingQueue = workingQueue;
            _mongoConnection = mongoConnection;
            _angleSharpParser = angleSharpParser;
            _inshakerClient = inshakerClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _workingQueue.RegisterConsumer<string[]>(ProcessDetailsBatch);
            _logger.LogInformation("Consumer successfully registered for queue");
        }

        private async Task<bool> ProcessDetailsBatch(string[] ids)
        {
            try
            {
                var batch = _mongoConnection.GetQuery<Cocktail>().Where(x => ids.Contains(x.Id)).OrderBy(x => x.Id)
                   .ToList();

                var tasks = new List<Task>();
                foreach (var item in batch)
                {
                    tasks.Add(Task.Run(async () => await FillDetailsAsync(item, CancellationToken.None)));
                }
                await Task.WhenAll(tasks);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _workingQueue.Dispose();
            return Task.CompletedTask;
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
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while processing cocktail id={0}", cocktail.Id);
            }
        }
    }
}
