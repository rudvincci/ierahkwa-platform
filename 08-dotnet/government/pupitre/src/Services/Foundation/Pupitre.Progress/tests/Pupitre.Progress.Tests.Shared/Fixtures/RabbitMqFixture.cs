using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mamey.MessageBrokers.RabbitMQ;
using Newtonsoft.Json;
using Pupitre.Progress.Tests.Shared.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pupitre.Progress.Tests.Shared.Fixtures
{
    public class RabbitMqFixture
    {
        private readonly IChannel _channel;
        private bool _disposed = false;
        
        public RabbitMqFixture()
        {
            var options = OptionsHelper.GetOptions<RabbitMqOptions>("rabbitMq");
            var connectionFactory = new ConnectionFactory
            {
                HostName = options.HostNames?.FirstOrDefault(),
                VirtualHost = options.VirtualHost,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password,
                // UseBackgroundThreadsForIO = true,
                // DispatchConsumersAsync = true,
                Ssl = new SslOption()
            };

            var connection = connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task PublishAsync<TMessage>(TMessage message, string exchange = null) where TMessage : class
        {
            var routingKey = SnakeCase(message.GetType().Name);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = new BasicProperties();
            properties.Headers = new Dictionary<string, object>();
            properties.MessageId = Guid.NewGuid().ToString();
            properties.CorrelationId = Guid.NewGuid().ToString();
       
            await _channel.BasicPublishAsync(exchange, routingKey, true ,properties, body);
            
        }
        
        public async Task<TaskCompletionSource<TEntity>> SubscribeAndGetAsync<TMessage, TEntity>(string exchange,
            Func<Guid, TaskCompletionSource<TEntity>, Task> onMessageReceived, Guid id)
        {
            var taskCompletionSource = new TaskCompletionSource<TEntity>();
            
            await _channel.ExchangeDeclareAsync(exchange: exchange,
                durable: true,
                autoDelete: false,
                arguments: null,
                type: "topic");

            var queue = $"test_{SnakeCase(typeof(TMessage).Name)}";

            await _channel.QueueDeclareAsync(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueBindAsync(queue, exchange, SnakeCase(typeof(TMessage).Name));
            await _channel.BasicQosAsync(0, 1, false);
            
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body;
                var json = Encoding.UTF8.GetString(body.Span);
                var message = JsonConvert.DeserializeObject<TMessage>(json);

                await onMessageReceived(id, taskCompletionSource);
            };
            
            await _channel.BasicConsumeAsync(queue: queue,
                autoAck: true,
                consumer: consumer);
            
            return taskCompletionSource;
        }
        
        private static string SnakeCase(string value)
            => string.Concat(value.Select((x, i) =>
                    i > 0 && value[i - 1] != '.' && value[i - 1] != '/' && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _channel.Dispose();
            }

            _disposed = true;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}