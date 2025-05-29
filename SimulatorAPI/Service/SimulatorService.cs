using CommonLibrary.Config;
using CommonLibrary.Interface;
using CommonLibrary.Models;
using Microsoft.Extensions.Options;

namespace SimulatorAPI.Service
{
    public class SimulatorService: BackgroundService
    {
        private readonly IKafkaProducerService _producer;
        private readonly AppSettings _settings;

        public SimulatorService(IKafkaProducerService producer, IOptions<AppSettings> options)
        {
            _producer = producer;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rnd = new Random();
            while (!stoppingToken.IsCancellationRequested)
            {
                var truckData = new TruckData
                {
                    TruckId = "TRUCK123",
                    Latitude = 28.7041 + rnd.NextDouble() * 0.01,
                    Longitude = 77.1025 + rnd.NextDouble() * 0.01,
                    Speed = rnd.Next(40, 100),
                    Timestamp = DateTime.UtcNow
                };

                await _producer.SendMessageAsync(_settings.Kafka.TopicName, truckData);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
