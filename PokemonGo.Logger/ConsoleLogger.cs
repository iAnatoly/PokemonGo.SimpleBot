using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Logger
{

    public class ConsoleLogger : ILogger
    {
        private LogLevel maxLogLevel;

        public ConsoleLogger(LogLevel maxLogLevel)
        {
            this.maxLogLevel = maxLogLevel;
        }

        public void Write(string message, LogLevel level = LogLevel.Info)
        {
            if (level > maxLogLevel)
                return;

            switch (level)
            {
                case LogLevel.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Info:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Debug:
                    System.Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            
            System.Console.WriteLine($"[{ DateTime.Now.ToString("HH:mm:ss")}] { message}");
        }
    }
}
