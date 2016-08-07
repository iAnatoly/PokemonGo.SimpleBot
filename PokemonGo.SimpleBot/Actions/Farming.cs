using POGOProtos.Map.Fort;
using PokemonGo.Logger;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.SimpleBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonGo.SimpleBot.Extensions;

namespace PokemonGo.SimpleBot.Actions
{
    class Farming
    {
        private readonly Client _client;
        private readonly Settings _clientSettings;

        public Farming(Client client, Settings settings)
        {
            _client = client;
            _clientSettings = settings;
        }

        public async Task<IEnumerable<FortData>> GetNearbyPokestops()
        {
            var mapObjects = await _client.Map.GetMapObjects();
            var pokeStops = mapObjects.Item1.MapCells
                .SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                .OrderBy(pokeStop => Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, pokeStop.Latitude, pokeStop.Longitude))
                .ToList();
            return pokeStops;
        }

        public async Task WalkToPokeStop(FortData pokeStop)
        {
            await _client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude, _clientSettings.DefaultAltitude, _clientSettings.SpeedKmh, pokeStop.Type.ToString());
        }

        public async Task FarmPokeStop(FortData pokeStop)
        {
            var fortInfo = await _client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
            var fortSearch = await _client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
            Log.Write($"Reached '{fortInfo.Name}'; XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}, Items: {StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded)}");
        }
    }
}
