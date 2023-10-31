using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using RabbitMQ.Client;
using System.Text;

namespace BusinessLayer.Services
{
    public class RabbitMqService : IQueueService
    {
        readonly string ExchangeName = "TestExchange";

        public async Task<SendResultModel> PostMessageAsync(SendRequestModel sendRequestModel)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };

                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);

                        var body = Encoding.UTF8.GetBytes(sendRequestModel.Message);

                        channel.BasicPublish(ExchangeName, "key", null, body);

                        return new SendResultModel { Result = $"Message was posted to {ExchangeName}" };
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
