using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PasswordChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            HashPassword();
        }

        static string Hash(string input)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        static void HashPassword()
        {
            string pass = "";            
            Console.Write("Enter your password: ");
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            if (pass != "q")
            {
                string hashed = Hash(pass);
                var hashList = DownloadHashList(hashed);
                var hashedSuffix = hashed.Substring(5, hashed.Length - 5).ToUpper();

                if (hashList.ContainsKey(hashedSuffix))
                {
                    Console.WriteLine($"Your password has been Pwned {hashList.GetValueOrDefault(hashedSuffix)} times");
                }
                else
                {
                    Console.WriteLine("Your password has not been Pwned");
                }
                          
                Console.WriteLine();
                Console.WriteLine("Enter q to exit");
                Console.WriteLine();
                HashPassword();
            }
        }

        private static Dictionary<string, string> DownloadHashList(string hashed)
        {
            string hashPrefix = hashed.Substring(0, 5);
            string url = $"https://api.pwnedpasswords.com/range/{hashPrefix}";
            var hashList = new Dictionary<string, string>();

            WebClient wc = new WebClient();
            wc.Headers.Add("User-Agent", "Pwnage-Checker-For-CIS");
            string[] hashListRaw = wc.DownloadString(url).Split("\r\n");

            foreach(string hashPair in hashListRaw)
            {
                string[] hash = hashPair.Split(":");
                hashList.Add(hash[0], hash[1]);
                
            }

            return hashList;
        }
    }
}
