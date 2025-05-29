using CommonLibrary.Config;
using CommonLibrary.Interface;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
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
        private readonly AppSettings configuration;

        public KafkaProducerService(IOptions<AppSettings> options)
        {
            // Initialize the Kafka producer with configuration settings from AppSettings
            configuration = options.Value;

            var config = new ProducerConfig
            {
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
            if (configuration.Kafka.AllowAutoCreateTopics)
            {
                await CreateTopicIfNotExistsAsync(topic);
            }

            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }

        public async Task CreateTopicIfNotExistsAsync(string topicName, int numPartitions = 3, short replicationFactor = 1)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = configuration.Kafka.BootstrapServers
            }).Build();

            try
            {
                await adminClient.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = numPartitions,
                        ReplicationFactor = replicationFactor
                    }
                });
                Console.WriteLine($"Topic {topicName} created successfully");
            }
            catch (CreateTopicsException e) when (e.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
            {
                Console.WriteLine($"Topic {topicName} already exists");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred creating topic {topicName}: {ex.Message}");
                throw;
            }
        }

    }
}
