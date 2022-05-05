//using AngleSharp.Parser;
//using Oxford.Client;
//using OxfordParser.Data;
//using OxfordParser.Data.Entities;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using MongoDB.Driver.Linq;
//using MongoDB.Driver;
//using Oxford.Client.Exceptions;
//using System.Collections.Concurrent;

//namespace OxfordParser
//{
//    public class WordListWorker : BackgroundService
//    {
//        private readonly ILogger<WordListWorker> _logger;
//        private readonly AngleSharpParser _parser;
//        private readonly MongoConnection _mongoConnection;
//        private int _currentPage = 0;
//        private readonly ConcurrentBag<int> _failedPages = new ConcurrentBag<int>();
//        public WordListWorker(
//            ILogger<WordListWorker> logger,
//            OxfordClient inshakerClient,
//            AngleSharpParser parser, 
//            MongoConnection mongoConnection)
//        {
//            _logger = logger;
//            _parser = parser;
//            _mongoConnection = mongoConnection;
//        }
//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var workersCount = 5;
//            _logger.LogInformation("Worker started with ParallelWorkersCount: {0}", workersCount);
//            await ProcessCocktailPages(workersCount, stoppingToken);
//        }
//        public override Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Worker is shutting down");

//            return base.StopAsync(cancellationToken);
//        }

//        private async Task ProcessCocktailPages(int parallelWorkers, CancellationToken cancellationToken)
//        {
//            var tasks = new List<Task>();
//            for (int i = 0; i < parallelWorkers; i++)
//            {
//                var page = _currentPage;
//                tasks.Add(ProcessPageAsync(page, cancellationToken));
//                _currentPage++;
//            }

//            try
//            {
//                await Task.WhenAll(tasks);
//            }
//            catch (EmptyResponseException)
//            {
//                _logger.LogInformation("Worker finished");
//                return;
//            }
//            await ProcessCocktailPages(parallelWorkers, cancellationToken);
//        }

//        private async Task ProcessPageAsync(int page, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var pageHtml = await _inshakerClient.GetPageAsync(page, cancellationToken);
//                var pageItems = await _parser.ParseCocktailsPage(pageHtml, cancellationToken);

//                var query = _mongoConnection.GetQuery<Cocktail>();
//                var collection = _mongoConnection.GetCollection<Cocktail>();

//                foreach (var item in pageItems)
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    var itemExists = await query.AnyAsync(x => x.Id == item.Id);
//                    if (itemExists)
//                        continue;

//                    await collection.InsertOneAsync(new Cocktail()
//                    {
//                        Id = item.Id,
//                        Processed = false,
//                        RelativeDetailsUrl = item.Link,
//                        Title = item.Title
//                    });
//                }
//            }
//            catch (EmptyCocktailListPageException)
//            {
//                throw;
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, "Page processing failed with exception");
//                _failedPages.Add(page);
//            }

//        }

       
//    }
//}
