using System.Security.Cryptography;

namespace GeneratePasswordTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int attempts = 1000;
            TimeSpan time1 = new TimeSpan();
            for (int i = 0; i < attempts; i++)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                GeneratePasswordHashUsingSalt($"passwordText + {i}", new byte[32]);
                stopwatch.Stop();
                time1 += stopwatch.Elapsed;;
            }
            Console.WriteLine($"Sum time1 = '{time1}'");
            var avgTime1 = time1 / attempts;
            Console.WriteLine($"Avg time1 = '{avgTime1}'");
            Console.Read();
        }

        public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }
    }
}