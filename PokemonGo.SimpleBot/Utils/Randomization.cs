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

        public static int GetRandomDelayForDistance(double distanceMeters, double speedKmh)
        {
            var speedms = speedKmh / 3.6;
            var maxSpeedDelay = (int) (distanceMeters/speedms*1000);
            var delay = Rng.Next(maxSpeedDelay, maxSpeedDelay*2+1);
            return delay;
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
