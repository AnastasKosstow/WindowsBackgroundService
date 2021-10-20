using RabbitMQ.Client;
using System.Configuration;
using System.Text.Json;

namespace WorkerService.RabbitMq
{
    public class WorkerServicePublisher : IWorkerServicePublisher
    {
        private readonly RabbitMqConfiguration _rabbitMq;

        public WorkerServicePublisher()
        {
            _rabbitMq = 
                ConfigurationManager.GetSection("RabbitMqConfiguration") as RabbitMqConfiguration;
        }

        public void Publish(object data)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(data);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMq.Hostname
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(_rabbitMq.QueueName, exclusive: false, autoDelete: false);
            channel.BasicPublish(string.Empty, _rabbitMq.QueueName, null, body);
        }
    }
}
