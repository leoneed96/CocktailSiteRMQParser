using OxfordParser.Data;
using OxfordParser.Data.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQTools.WorkingQueue;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordParser
{
    public class DetailsWorker : BackgroundService
    {
        private readonly MongoConnection _mongoConnection;
        private readonly ILogger<DetailsWorker> _logger;
        private readonly WorkingQueueWrapper _queueWrapper;
        private Timer _processTimer;
        private List<Process> _workerProcesses = new List<Process>();
        public DetailsWorker(
            MongoConnection mongoConnection,
            ILogger<DetailsWorker> logger,
            WorkingQueueWrapper queueWrapper)
        {
            _mongoConnection = mongoConnection;
            _logger = logger;
            _queueWrapper = queueWrapper;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var batchSize = 3;
            _processTimer = new Timer(async (a) => await DistributeBatchAsync(batchSize, stoppingToken),
                state: null,
                dueTime: 0,
                period: Timeout.Infinite);

            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var item in _workerProcesses)
            {
                item.Close();
            }
            _queueWrapper.Dispose();
            return Task.CompletedTask;
        }
        private async Task DistributeBatchAsync(int batchSize, CancellationToken cancellationToken)
        {
            var batch = _mongoConnection.GetQuery<Cocktail>().Where(x => !x.Processed && !x.Processing)
                .OrderBy(x => x.Id)
                .Take(batchSize)
                .Select(x => x.Id)
                .ToList();
            if(!batch.Any())
            {
                // set another request after 10 seconds to check if we have something to process
                _processTimer.Change(dueTime: 10000, period: Timeout.Infinite);
                return;
            }
            var update = Builders<Cocktail>.Update
                  .Set(x => x.Processing, true);
            var filter = Builders<Cocktail>.Filter
                .In(x => x.Id, batch);

            var updateResult = await _mongoConnection.GetCollection<Cocktail>().UpdateManyAsync(filter, update);

            await _queueWrapper.PushToQueue(batch.ToArray());


            // manually start timer again 
            _processTimer.Change(dueTime: 0, period: Timeout.Infinite);
        }

        string FindWorkerPath(DirectoryInfo rootDir)
        {
            var workerFolder = rootDir.GetDirectories().FirstOrDefault(x => x.Name == "RMQWorkerFolder");
            if (workerFolder != null)
            {
                return workerFolder.FullName;
            }

            return FindWorkerPath(rootDir.Parent);
        }

    }
}
