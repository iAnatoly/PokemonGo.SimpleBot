using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.Logger;

namespace PokemonGo.SimpleBot
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            Log.SetLogger(new ConsoleLogger(LogLevel.Info));

            var settings = new Settings()
            {
                GoogleUsername = Utils.LoginPassword.GetUsername(),
                GooglePassword = Utils.LoginPassword.GetPassword()
            };

            Task.Run(() =>
            {
                try
                {
                    new Logic(settings).Execute().Wait();
                }
                catch (Exception ex)
                {
                    Log.Write($"Unhandled exception: {ex}", LogLevel.Error);
                }
            });
            System.Console.ReadLine();
        }
    }
}
