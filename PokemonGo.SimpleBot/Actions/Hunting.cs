using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.Logger;
using PokemonGo.RocketAPI;
using PokemonGo.SimpleBot.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static POGOProtos.Networking.Responses.CatchPokemonResponse.Types;

namespace PokemonGo.SimpleBot.Actions
{
    class Hunting
    {
        private readonly Client _client;
        private readonly Settings _clientSettings;

        public Hunting(Client client, Settings settings)
        {
            _client = client;
            _clientSettings = settings;
        }

        static readonly List<ItemId> _pokeballs = new List<ItemId>()
        {
            ItemId.ItemMasterBall,
            ItemId.ItemUltraBall,
            ItemId.ItemGreatBall,
            ItemId.ItemPokeBall,
        };

        private async Task<ItemId> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var inventoryItems = await _client.Inventory.GetInventoryItems();

            var balls = inventoryItems
                .Where(i => i.Item != null)
                .Where(i => _pokeballs.Contains(i.Item.ItemId) && i.Item?.Count > 0)
                .Select(i => i.Item)
                .ToDictionary(i => i.ItemId);

            if (pokemonCp > 1000)
                foreach (var ball in _pokeballs)
                    if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            if (pokemonCp > 600)
                foreach (var ball in _pokeballs.Skip(1))
                    if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            if (pokemonCp > 350)
                foreach (var ball in _pokeballs.Skip(2))
                    if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            foreach (var ball in _pokeballs.Reverse<ItemId>())
                if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            throw new Exceptions.OutOfPokeBallsException();
        }

        public async Task<IEnumerable<MapPokemon>> GetNearbyPokemons()
        {
            var mapObjects = await _client.Map.GetMapObjects();

            var pokemons = mapObjects.Item1.MapCells
                .SelectMany(i => i.CatchablePokemons)
                .OrderBy(p => Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, p.Latitude, p.Longitude))
                .ToList();

            return pokemons;
        }

        internal async Task<EncounterResponse> WalkToPokemon(MapPokemon pokemon)
        {
            await _client.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude, _clientSettings.DefaultAltitude, _clientSettings.SpeedKmh, pokemon.PokemonId.ToString());
            var encounter = await _client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
            return encounter;
        }

        public async Task CatchEncounter(EncounterResponse encounter, MapPokemon pokemon)
        {

            var probability = encounter?.CaptureProbability?.CaptureProbability_;
            if (probability == null) return;

            foreach (var p in probability)
            { 
                var pokeball = await GetBestBall(encounter.WildPokemon);

                if (p < 0.35)
                {
                    await _client.Encounter.UseCaptureItem(pokemon.EncounterId, ItemId.ItemRazzBerry, pokemon.SpawnPointId);
                    await Randomization.RandomDelay(2000);
                }
             
                var response = await _client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball);                
                await Randomization.RandomDelay(2000);

                if (response.Status == CatchStatus.CatchMissed || response.Status == CatchStatus.CatchEscape) continue;
                
                Log.Write($" {((response.Status == CatchStatus.CatchSuccess) ? '+' : '-')} {pokemon.PokemonId}: CP {encounter.WildPokemon?.PokemonData?.Cp}; P: {p} [{pokeball}]");
                break;
            } 
        }
    }
}
