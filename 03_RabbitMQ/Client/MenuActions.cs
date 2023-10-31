using BusinessLayer.Models;
using Client.Helpers;

namespace Client
{
    public class MenuActions
    {
        static HttpClient? httpClient { get; set; }

        public MenuActions()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7151/api/");
        }

        public Dictionary<int, Func<Task>> Actions = new()
        {
            [0] = SendDateTimeMessageAsync,
        };

        public static async Task SendDateTimeMessageAsync()
        {
            var message = $"New Message '{DateTime.UtcNow}'";
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"Queue/PostMessageToQueue").AddContent(new SendRequestModel { Message = message }));
            var responseModel = await responseMessage.GetModelAsync<SendResultModel>();

            PrintColored($"\n{responseModel?.Result}", ConsoleColor.Green);
        }
       
        public void Greetings()
        {
            Console.WriteLine("\n*******************************************************************************");
            Console.WriteLine("*                                                                             *");
            Console.WriteLine("*                             WELCOME TO COOL                                 *");
            Console.WriteLine("*                            RABBITMQ SYSTEM                                  *");
            Console.WriteLine("*                                                                             *");
            Console.WriteLine("*******************************************************************************\n");
        }

        public static void PrintColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
