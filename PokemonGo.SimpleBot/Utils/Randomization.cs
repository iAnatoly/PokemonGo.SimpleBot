using System;
using System.Threading.Tasks;
using PokemonGo.Logger;

namespace PokemonGo.SimpleBot.Utils
{
    internal static class Randomization
    {

        private static readonly Random Rng = new Random();

        public static async Task RandomDelay(int maxDelay)
        {
            var delay = Rng.Next((maxDelay > 500) ? 500 : 0, maxDelay);
            await Task.Delay(delay);
        }

        public static async Task RandomDelayDistnace(double distanceMeters, double speedKmh, int delayCap=60000)
        {
            var speedms = speedKmh / 3.6;
            var maxDelay = (int) (distanceMeters/speedms*1000);
            var delay = Rng.Next(maxDelay, maxDelay*2+1);
            if (delay > delayCap) delay = Rng.Next(delayCap/2, delayCap); 
            Log.Write($"It takes me {Math.Round(delay/1000.0)}s to walk {Math.Round(distanceMeters)}m. Actual speed: {Math.Round(distanceMeters*60*60/delay)}km/h");
            await Task.Delay(delay);
        }

        public static double GetRandomizedCoordinate(double coordinate)
        {
            return coordinate + 0.00010 * Rng.NextDouble();
        }

        public static int GetRandomizedWeight(double distance)
        {
            return (int) Math.Round(distance / 10)*10 + Rng.Next(10);
        }
    }
}
