using AngleSharp.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Oxford.Client;
using OxfordParser.Data;
using OxfordParser.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordParser
{
    public class WordListWorker : BackgroundService
    {
        private readonly ILogger<WordListWorker> _logger;
        private readonly IDbContextFactory<WordsDbContext> _dbContextFactory;
        private readonly FileStorageService _fileStorageService;
        private readonly WordDetailsParser _wordDetailsParser;
        private readonly WordListPageParser _wordListPageParser;
        private readonly OxfordClient _oxfordClient;
        private HashSet<WordEntry> _wordEntries;
        public WordListWorker(
            ILogger<WordListWorker> logger,
            IDbContextFactory<WordsDbContext> dbContextFactory,
            FileStorageService fileStorageService,
            WordDetailsParser wordDetailsParser,
            WordListPageParser wordListPageParser,
            OxfordClient oxfordClient)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _fileStorageService = fileStorageService;
            _wordDetailsParser = wordDetailsParser;
            _wordListPageParser = wordListPageParser;
            _oxfordClient = oxfordClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started");
            foreach (var item in Directory.GetFiles("Lists", "*.html"))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested, stopping files processing");
                    break;
                }

                _logger.LogInformation("Working with list {0}", item);
                using(var dbContext = _dbContextFactory.CreateDbContext())
                {
                    _wordEntries = dbContext.Words.AsNoTracking().Select(x => new WordEntry()
                    {
                        Text = x.Text,
                        Type = x.WordType.NameEng
                    }).ToHashSet();
                }
                await ProcessList(File.ReadAllText(item), stoppingToken);
            }

            _logger.LogInformation("Worker finished");
        }

        private async Task ProcessList(string listHtml, CancellationToken cancellationToken)
        {
            foreach (var item in _wordListPageParser.GetWordListItems(listHtml, cancellationToken))
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Cancellation requested, stopping item processing");
                        break;
                    }

                    var entry = new WordEntry()
                    {
                        Text = item.WordText,
                        Type = item.Type
                    };
                    if (_wordEntries.Contains(entry))
                    {
                        _logger.LogInformation("Item {0} already exists in database, skipping", item.WordText);
                        continue;
                    }

                    _logger.LogInformation("Processing list item {0}", item.WordText);
                    using var dbContext = _dbContextFactory.CreateDbContext();
                    
                    if (await WordsRepository.ExistsAsync(dbContext, item))
                    {
                        _logger.LogInformation("Item {0} already exists in database, skipping", item.WordText);
                        continue;
                    }

                    using var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                    var detailsHtml = await _oxfordClient.GetDetailsAsync(item.DetailsLink, cancellationToken);
                    var itemDetails = await _wordDetailsParser.GetWordDetails(detailsHtml);

                    if (!_fileStorageService.TryGetExistedSoundPath(item.WordText, VoiceType.UK, out var ukPath))
                    {
                        if (!string.IsNullOrEmpty(item.UKSoundLink))
                        {
                            var ukFile = await _oxfordClient.GetSoundSteamAsync(item.UKSoundLink, cancellationToken);
                            ukPath = await _fileStorageService.SaveSoundAsync(item.WordText, ukFile, VoiceType.UK);
                        }
                    }

                    if (!_fileStorageService.TryGetExistedSoundPath(item.WordText, VoiceType.US, out var usPath))
                    {
                        if (!string.IsNullOrEmpty(item.AmericanSoundLink))
                        {
                            var usFile = await _oxfordClient.GetSoundSteamAsync(item.AmericanSoundLink, cancellationToken);
                            usPath = await _fileStorageService.SaveSoundAsync(item.WordText, usFile, VoiceType.US);
                        }
                    }

                    var selectedUsages = WordUsagesSelector.SelectUsagesFromDetails(itemDetails.Usages, 5);
                    var wordType = await WordsRepository.GetOrAddWordTypeAsync(dbContext, item.Type);
                    var word = await WordsRepository.CreateWordAsync(dbContext, item, itemDetails, wordType, ukPath, usPath, selectedUsages);

                    transaction.Commit();
                }
                catch(Exception e)
                {
                    _logger.LogCritical(e, "An error occured while processing word {0}", item.WordText);
                    Console.Beep();
                    continue;
                }
            }
        }

        private struct WordEntry
        {
            public string Text { get; set; }
            public string Type { get; set; }
        }
    }
}