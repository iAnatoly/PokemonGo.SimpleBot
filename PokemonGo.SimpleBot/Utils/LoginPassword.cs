using System;
using System.ComponentModel;
using System.Security;
using System.Text;

namespace PokemonGo.SimpleBot.Utils
{
    static class LoginPassword
    {
        public static string GetUsername()
        {
            System.Console.Write("u: ");
            return System.Console.ReadLine()?.Trim();
        }


        public static string GetPassword()
        {
            System.Console.Write("p: ");
            var pwd = new StringBuilder();
            while (true)
            {
                var readkey = System.Console.ReadKey(true);
                if (readkey.Key == ConsoleKey.Enter) break;
                if (readkey.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length == 0) continue;

                    pwd.Length--;
                    System.Console.Write("\b \b");
                }
                else
                {
                    pwd.Append(readkey.KeyChar);
                    System.Console.Write("*");
                }
            }
            System.Console.WriteLine();
            return pwd.ToString();
        }
    }
}
