using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var menu = new MenuActions();
            menu.Greetings();

            var host = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<SignalRListenerService>();
            })
            .Build();

            await host.StartAsync();

            while (true)
            {
                Console.WriteLine("\n0. Send messge to RabbitMQ");
                Console.WriteLine("1. Send file to RabbitMQ");
                Console.WriteLine("2. Exit the program\n");
                Console.Write("Enter your choice:\n");

                if (int.TryParse(Console.ReadLine(), out var choiceInt) && choiceInt >= 0 && choiceInt <= 2)
                {
                    if (choiceInt == 2)
                    {
                        // Stop the host before exiting the program
                        await host.StopAsync();
                        Environment.Exit(0);
                    }

                    _ = menu.Actions[choiceInt]();
                }
                else
                {
                    MenuActions.PrintColored("Invalid choice. Please enter a number between 0 and 2.", ConsoleColor.Yellow);
                }
            }
        }
    }
}