using CommonLibrary.Interface;
using CommonLibrary.Models;

namespace SimulatorAPI.Service
{
    public class SimulatorService: BackgroundService
    {
        private readonly IKafkaProducerService _producer;
        public SimulatorService(IKafkaProducerService producer)
        {
            _producer = producer;
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

                await _producer.SendMessageAsync("truck-topic", truckData);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
