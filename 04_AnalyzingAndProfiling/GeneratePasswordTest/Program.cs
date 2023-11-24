using System.Diagnostics;
using System.Security.Cryptography;

namespace GeneratePasswordTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ORIGINAL GeneratePasswordHashUsingSalt START");
            ExecutePasswordGeneration(GeneratePasswordHashUsingSalt);
            Console.WriteLine("ORIGINAL GeneratePasswordHashUsingSalt END\n");

            Console.WriteLine("UPD1 GeneratePasswordHashUsingSalt1 START");
            ExecutePasswordGeneration(GeneratePasswordHashUsingSalt1);
            Console.WriteLine("UPD1 GeneratePasswordHashUsingSalt1 END\n");

            Console.WriteLine("UPD2 GeneratePasswordHashUsingSalt2 START");
            ExecutePasswordGeneration(GeneratePasswordHashUsingSalt2);
            Console.WriteLine("UPD2 GeneratePasswordHashUsingSalt2 END\n");

            Console.Read();
        }

        private static void ExecutePasswordGeneration(Func<string, byte[], string> func)
        {
            int attempts = 100000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    var filestream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestFile.txt"), FileMode.Open);
                    var byteArray = new byte[filestream.Length];
                    filestream.Read(byteArray, 0, byteArray.Length);
                }
                catch (Exception)
                {
                }

                func($"passwordText{i}", new byte[32]);
            }

            stopwatch.Stop();

            Console.WriteLine($"Total time = '{stopwatch.Elapsed}'");
            var avgTime = stopwatch.Elapsed / attempts;
            Console.WriteLine($"Avg time = '{avgTime}'");
        }

        public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {

            var iterate = 100000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;

        }

        public static string GeneratePasswordHashUsingSalt1(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[salt.Length + hash.Length];

                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string GeneratePasswordHashUsingSalt2(string passwordText, byte[] salt)
        {
            var iterate = 100000;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate))
            {
                byte[] hash = pbkdf2.GetBytes(20);

                Span<byte> hashBytes = stackalloc byte[salt.Length + hash.Length];
                salt.CopyTo(hashBytes);
                hash.CopyTo(hashBytes.Slice(salt.Length));

                return Convert.ToBase64String(hashBytes.ToArray());
            }
        }
    }
}
