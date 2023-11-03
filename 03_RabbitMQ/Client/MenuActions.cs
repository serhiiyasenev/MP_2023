using BusinessLayer.Models;
using Client.Helpers;
using System.Net.Http.Headers;

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
            [1] = SendPdfFileAsync,
        };

        public static async Task SendDateTimeMessageAsync()
        {
            var message = $"New Message '{DateTime.UtcNow}'";
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"Queue/PostMessageToQueue").AddContent(new SendRequestModel { Message = message }));
            var responseModel = await responseMessage.GetModelAsync<SendResultModel>();

            PrintColored($"\n{responseModel?.Result}", ConsoleColor.Green);
        }

        public static async Task SendPdfFileAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "1_KB_FILE.pdf");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var fileStreamContent = new StreamContent(fileStream);
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            var content = new MultipartFormDataContent { { fileStreamContent, "file", Path.GetFileName(filePath) } };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Queue/PostFileToQueue");
            requestMessage.Content = content;

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseModel = await responseMessage.GetModelAsync<SendResultModel>();
                PrintColored($"\n{responseModel?.Result}", ConsoleColor.Green);
            }
            else
            {
                var errorResponse = await responseMessage.Content.ReadAsStringAsync();
                PrintColored($"\nError: {errorResponse}", ConsoleColor.Red);
            }
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
