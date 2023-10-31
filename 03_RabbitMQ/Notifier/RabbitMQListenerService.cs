using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Notifier
{
    public class RabbitMQListenerService : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public RabbitMQListenerService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "TestQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _ = _hubContext.Clients.All.SendAsync("ReceiveMessage", "RabbitMQ Default User", $"Task \"{message}\" was marked as Done. Great job!");
            };

            channel.BasicConsume(queue: "TestQueue", autoAck: true, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

}
