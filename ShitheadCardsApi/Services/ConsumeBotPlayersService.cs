using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShitheadCardsApi.Interfaces;

using System;
using System.Threading;
using System.Threading.Tasks;

using GrpcPromWrite;
using System.Net.Http;
using Google.Protobuf;
using System.IO;
using System.Net.Http.Headers;
using Snappy;
using System.Collections.Generic;
using ShitheadCardsApi.Models;
using Microsoft.Extensions.Configuration;

namespace ShitheadCardsApi
{
    public class ConsumeBotPlayersService : IHostedService
    {
        private Timer timer;
        private IConfiguration Configuration;
        public IServiceProvider Services { get; }

        public ConsumeBotPlayersService(IServiceProvider services, IConfiguration configuration)
        {
            Configuration = configuration;
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
                monitorGames(gameService.List(null, false));
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


        private void monitorGames(List<Game> games)
        {
            var cntSetup = games.FindAll(game => game.Status == StatusEnum.SETUP).Count;

            var cntPlaying = games.FindAll(game => game.Status == StatusEnum.PLAYING).Count;

            var cntOut = games.FindAll(game => game.Status == StatusEnum.OUT).Count;

            var timeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var req = new WriteRequest
            {
                Timeseries = {
                    new TimeSeries {
                        Samples = {
                            new Sample {
                                Value = cntSetup,
                                Timestamp = timeNow
                            }
                        },
                        Labels = {
                            new Label { Name = "__name__", Value = "sh_game_count" },
                            new Label { Name = "game_status", Value = "SETUP"}
                        }
                    },
                    new TimeSeries {
                        Samples = {
                            new Sample {
                                Value = cntPlaying,
                                Timestamp = timeNow
                            }
                        },
                        Labels = {
                            new Label { Name = "__name__", Value = "sh_game_count" },
                            new Label { Name = "game_status", Value = "PLAYING"}
                        }
                    },
                    new TimeSeries {
                        Samples = {
                            new Sample {
                                Value = cntOut,
                                Timestamp = timeNow
                            }
                        },
                        Labels = {
                            new Label { Name = "__name__", Value = "sh_game_count" },
                            new Label { Name = "game_status", Value = "OUT"}
                        }
                    }
                }
            };

            SendMetricRequest(req);
        }


        private void SendMetricRequest(WriteRequest req)
        {

            var url = Configuration.GetValue<string>("prom_write_url");
            var token = Configuration.GetValue<string>("prom_write_token");


            if (! String.IsNullOrEmpty(url) )
            { 
                MemoryStream stream = new MemoryStream();

                req.WriteTo(stream);
                byte[] arrIn = stream.ToArray();

                byte[] arr = SnappyCodec.Compress(arrIn);

                var handler = new HttpClientHandler();

                var httpClient = new HttpClient(handler);

                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("POST"), url);

                requestMessage.Headers.Add("Authorization", $"Bearer {token}");

                requestMessage.Content = new ByteArrayContent(arr);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
                requestMessage.Content.Headers.ContentEncoding.Add("snappy");


                HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;

            }
        }

    }
}
