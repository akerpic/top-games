using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TopGames.EntityFramework;
using TopGames.Models;

namespace TopGames.Helpers
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IStoreCrawler _store;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;
        public IServiceScopeFactory _serviceScopeFactory;


        public TimedHostedService(ILogger<TimedHostedService> logger, IStoreCrawler store, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _store = store;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }

        private async void SaveGameToDb(Game game)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<HerokuDbContext>();
                try
                {
                    context.game.Add(game);
                    await context.SaveChangesAsync();
                    _logger.LogInformation(game.title + " is saved to DB");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

        }

        private async void DoWork(object state)
        {
            DateTime start = DateTime.Now;
            _logger.LogInformation("Start Time:" + string.Format("{0:s}", start));
            var topGames = await _store.GetTopGames();
            foreach (var game in topGames)
            {
                SaveGameToDb(game);
            }
            DateTime end = DateTime.Now;
            _logger.LogInformation("End Time:" + string.Format("{0:s}", end));
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
