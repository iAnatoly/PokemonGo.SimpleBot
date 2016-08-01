using PokemonGo.Logger;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Rpc;
using PokemonGo.SimpleBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.Actions
{
    static class ClientRandomizationExtension
    {
        public static async Task UpdatePlayerLocation(this Client client, double latitude, double longitude, double altitude, double speedKmh, string destination)
        {
            var randomizedLatitude = Randomization.GetRandomizedCoordinate(latitude);
            var randomizedLongitude = Randomization.GetRandomizedCoordinate(longitude);
            var distance = Navigation.DistanceBetween2Coordinates(client.CurrentLatitude, client.CurrentLongitude, latitude, longitude);
            var delay = Randomization.GetRandomDelayForDistance(distance, speedKmh);

            Log.Write($" . Walking towards {destination} for {Math.Round(distance)}m in {Math.Round(delay / 1000.0)}s @ {Math.Round(distance * 60 * 60 / delay)}km/h");
            await Task.Delay(delay);

            await client.Player.UpdatePlayerLocation(randomizedLatitude, randomizedLongitude, altitude);
            await Task.Delay(1000);
        }
    }
}
