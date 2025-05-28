using CommonLibrary.Config;
using CommonLibrary.Interface;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(AppSettings configuration)
        {
            var config = new ProducerConfig { 
                BootstrapServers = configuration.Kafka.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true, // Ensure exactly-once delivery, no duplicate messages.
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 100,
                LingerMs = 5,
                CompressionType = CompressionType.Snappy // Use Snappy compression for better performance
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task SendMessageAsync<T>(string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }
    }
}
