using System;
using System.Text;

namespace PokemonGo.SimpleBot.Utils
{
    static class LoginPassword
    {
        public static string GetUsername()
        {
            Console.Write("u: ");
            return Console.ReadLine()?.Trim();
        }


        public static string GetPassword()
        {
            Console.Write("p: ");
            var pwd = new StringBuilder();
            while (true)
            {
                var readkey = Console.ReadKey(true);
                if (readkey.Key == ConsoleKey.Enter) break;
                if (readkey.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length == 0) continue;

                    pwd.Length--;
                    Console.Write("\b \b");
                }
                else
                {
                    pwd.Append(readkey.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd.ToString();
        }
    }
}
