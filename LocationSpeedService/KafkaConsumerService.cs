using CommonLibrary.Config;
using CommonLibrary.Models;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocationSpeedService
{
    internal class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly AppSettings _settings;
        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, AppSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _settings.Kafka.BootstrapServers,
                GroupId = _settings.Kafka.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = _settings.Kafka.AllowAutoCreateTopics
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_settings.Kafka.TopicName);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                var truckData = JsonSerializer.Deserialize<TruckData>(result.Message.Value);

                _logger.LogInformation($"Truck {truckData.TruckId} - Speed: {truckData.Speed} km/h, Location: ({truckData.Latitude}, {truckData.Longitude}) at {truckData.Timestamp}");
            }
            return Task.CompletedTask;
        }
    }
}
