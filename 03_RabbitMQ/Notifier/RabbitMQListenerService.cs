using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Notifier
{
    public class RabbitMQListenerService : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly string _amqpUrl;
        private readonly string _queueName;
        private readonly string _downloadedFilesFolder;

        public RabbitMQListenerService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _amqpUrl = "amqp://guest:guest@localhost:5672";
            _queueName = "fileQueue";
            _downloadedFilesFolder = Path.Combine(Directory.GetCurrentDirectory(), "DownloadedFiles");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_amqpUrl) };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, message) =>
            {
                var headers = message.BasicProperties.Headers;
                if (headers != null)
                {
                    var fileId = Encoding.UTF8.GetString((byte[])headers["FileId"]);
                    var fileName = Encoding.UTF8.GetString((byte[])headers["FileName"]);
                    var fileSizeBytes = long.Parse(headers["FileSizeBytes"].ToString());
                    var uploadTime = DateTime.Parse(Encoding.UTF8.GetString((byte[])headers["UploadTime"]));

                    var fileContent = message.Body.ToArray();
                    if (!Directory.Exists(_downloadedFilesFolder)) Directory.CreateDirectory(_downloadedFilesFolder);
                    var filePath = Path.Combine(_downloadedFilesFolder, fileName);
                    File.WriteAllBytes(filePath, fileContent);

                    _ = _hubContext.Clients.All.SendAsync("ReceiveMessage", "RabbitMQ Default User", 
                        $"File was received!" +
                        $"\n\"FileId: {fileId}\"" +
                        $"\n\"FileName: {fileName}\"" +
                        $"\n\"FileSize: {fileSizeBytes/1024} KB\"" +
                        $"\n\"UploadTime: {uploadTime}\""); 
                }
                else
                {
                    _ = _hubContext.Clients.All.SendAsync("ReceiveMessage", "RabbitMQ Default User", $"File without Headers was received!");
                }
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
