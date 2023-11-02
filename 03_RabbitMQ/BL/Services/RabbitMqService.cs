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
        readonly string FileRoutingKey = "fileQueue";
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

        public async Task<SendResultModel> PostFileAsync(string fileName,Stream fileStream)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var factory = new ConnectionFactory { Uri = new Uri(AmqpUri) };

                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);

                        channel.QueueDeclare(queue: FileRoutingKey,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        IBasicProperties properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        properties.Headers = new Dictionary<string, object>
                        {
                            { "FileId", Guid.NewGuid().ToString()         },
                            { "FileName", fileName                        },
                            { "UploadTime", DateTime.UtcNow.ToString("o") }
                        };

                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.Seek(0, SeekOrigin.Begin);
                            await fileStream.CopyToAsync(memoryStream);
                            var fileContent = memoryStream.ToArray();
                            channel.BasicPublish(exchange: ExchangeName,
                                                 routingKey: FileRoutingKey,
                                                 basicProperties: properties,
                                                 body: fileContent);
                        }

                        return new SendResultModel { Result = $"File was posted to {ExchangeName}/{FileRoutingKey}" };
                    }
                }
                catch (Exception ex)
                {
                    return new SendResultModel { Result = $"RabbitMqService error: {ex.Message}" };
                }
            });
        }

        public async Task<List<FileInfoModel>> GetUploadedFilesInfoAsync()
        {
            return await Task.Run(() =>
            {
                var fileInfoList = new List<FileInfoModel>();

                try
                {
                    var factory = new ConnectionFactory { Uri = new Uri(AmqpUri) };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        var messageCount = channel.MessageCount(FileRoutingKey);

                        for (int i = 0; i < messageCount; i++)
                        {
                            var message = channel.BasicGet(FileRoutingKey, autoAck: false);
                            if (message == null)
                            {
                                break;
                            }

                            var headers = message.BasicProperties.Headers;
                            if (headers != null)
                            {
                                fileInfoList.Add(new FileInfoModel
                                {
                                    FileId = Encoding.UTF8.GetString((byte[])headers["FileId"]),
                                    FileName = Encoding.UTF8.GetString((byte[])headers["FileName"]),
                                    UploadTime = DateTime.Parse(Encoding.UTF8.GetString((byte[])headers["UploadTime"]))
                                });
                            }
                            else
                            {
                                fileInfoList.Add(new FileInfoModel
                                {
                                    FileId = null,
                                    FileName = null,
                                    UploadTime = null
                                });
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving file information: {ex.Message}");
                }

                return fileInfoList;
            });
        }

        public async Task<(string FileName, byte[] FileContent)> GetFileByIdAsync(string fileId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var factory = new ConnectionFactory { Uri = new Uri(AmqpUri) };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        var messageCount = channel.MessageCount(FileRoutingKey);

                        for (int i = 0; i < messageCount; i++)
                        {
                            var message = channel.BasicGet(FileRoutingKey, autoAck: false);
                            if (message == null)
                            {
                                break;
                            }

                            var headers = message.BasicProperties.Headers;
                            if (headers != null && Encoding.UTF8.GetString((byte[])headers["FileId"]) == fileId)
                            {
                                // Acknowledge the message so it gets removed from the queue
                                channel.BasicAck(message.DeliveryTag, multiple: false);

                                return (FileName: Encoding.UTF8.GetString((byte[])headers["FileName"]), FileContent: message.Body.ToArray());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving file: {ex.Message}");
                }

                return (FileName: null, FileContent: null);
            });
        }

    }
}
