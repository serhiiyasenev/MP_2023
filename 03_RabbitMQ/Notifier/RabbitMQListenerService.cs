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
        private readonly string[] _queueNames;
        private readonly string _downloadedFilesFolder;
        private IConnection? _connection;
        private IList<IModel> _channels;

        public RabbitMQListenerService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _amqpUrl = "amqp://guest:guest@localhost:5672";
            _queueNames = new[] { "fileQueue", "TestQueue" };
            _downloadedFilesFolder = Path.Combine(Directory.GetCurrentDirectory(), "DownloadedFiles");
            _channels = new List<IModel>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_amqpUrl) };
            _connection = factory.CreateConnection();

            foreach (var queueName in _queueNames)
            {
                var channel = _connection.CreateModel();
                _channels.Add(channel);
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                
                var consumer = new EventingBasicConsumer(channel);
                
                consumer.Received += async (model, ea) => await ProcessMessageAsync(ea);
                
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var channel in _channels)
            {
                channel?.Close();
            }
            _connection?.Close();
            return base.StopAsync(cancellationToken);
        }

        private async Task ProcessMessageAsync(BasicDeliverEventArgs message)
        {
            await Task.Run(() =>
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
                        $"\n\"FileSize: {fileSizeBytes / 1024} KB\"" +
                        $"\n\"UploadTime: {uploadTime}\"");
                }
                else
                {
                    var messageText = Encoding.UTF8.GetString(message.Body.ToArray());
                    _ = _hubContext.Clients.All.SendAsync("ReceiveMessage", "RabbitMQ Default User", $"{messageText}");
                }
            });
        }
    }
}
