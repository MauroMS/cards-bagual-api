using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShitheadCardsApi.Interfaces;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShitheadCardsApi
{
    public class ConsumeBotPlayersService : IHostedService
    {
        private Timer timer;
        public IServiceProvider Services { get; }

        public ConsumeBotPlayersService(IServiceProvider services)
        {
            Services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(5);

            timer = new Timer(DoWork, null, startTimeSpan, periodTimeSpan);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = Services.CreateScope())
            {
                var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
                gameService.PlayBotTurns();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

    }
}
