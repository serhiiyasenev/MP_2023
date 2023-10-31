using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using RabbitMQ.Client;
using System.Text;

namespace BusinessLayer.Services
{
    public class RabbitMqService : IQueueService
    {
        readonly string ExchangeName = "TestExchange";
        readonly string RoutingKey = "key";
        readonly string AmqpUri = "amqp://guest:guest@localhost:5672";

        public async Task<SendResultModel> PostMessageAsync(SendRequestModel sendRequestModel)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var factory = new ConnectionFactory { Uri = new Uri(AmqpUri) };

                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);

                        var body = Encoding.UTF8.GetBytes(sendRequestModel.Message);

                        channel.BasicPublish(ExchangeName, RoutingKey, null, body);

                        return new SendResultModel { Result = $"Message was posted to '{ExchangeName}' for all Queues with RoutingKey '{RoutingKey}'" };
                    }
                }
                catch (Exception ex)
                {
                    return new SendResultModel { Result = $"RabbitMqService error: {ex.Message}" };
                }
            });
        }

        public async Task<SendResultModel> PostFileAsync(Stream fileStream)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var factory = new ConnectionFactory { Uri = new Uri(AmqpUri) };

                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);

                        var queueName = "fileQueue";
                        channel.QueueDeclare(queue: queueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.Seek(0, SeekOrigin.Begin);
                            fileStream.CopyToAsync(memoryStream);
                            var fileContent = memoryStream.ToArray();
                            channel.BasicPublish(exchange: ExchangeName,
                                                 routingKey: queueName,
                                                 basicProperties: properties,
                                                 body: fileContent);
                        }

                        return new SendResultModel { Result = $"File was posted to {ExchangeName}/{queueName}" };
                    }
                }
                catch (Exception ex)
                {
                    return new SendResultModel { Result = $"RabbitMqService error: {ex.Message}" };
                }
            });
        }
    }
}
