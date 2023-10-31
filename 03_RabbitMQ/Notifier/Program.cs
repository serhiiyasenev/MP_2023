using Notifier;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<RabbitMQListenerService>();
builder.Services.AddSignalR();
var app = builder.Build();

app.MapHub<ChatHub>("/chathub");

app.MapGet("/", () => "Hello, world! SignalR - Chathub is working now!");

app.Run();
