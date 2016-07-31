using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using POGOProtos.Enums;
using PokemonGo.Logger;

namespace PokemonGo.SimpleBot
{
    using POGOProtos.Data;
    using POGOProtos.Map.Fort;
    using POGOProtos.Map.Pokemon;
    using POGOProtos.Settings.Master;
    using RocketAPI.Extensions;
    using RocketAPI.Logic.Utils;
    using Utils;

    partial class Logic
    {
        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve()
        {
            var inventoryItems = await GetInventoryItems();
            var playerPokemons = inventoryItems
                .Select(i => i.PokemonData)
                .Where(p => p != null && p.PokemonId > 0);
            var pokemons = playerPokemons
                .Where(p => p.DeployedFortId != null) //Don't evolve pokemon in gyms

                .OrderByDescending(p => p.Cp)
                .ToList();
            return pokemons;
        }

        internal async Task EvolveAllPokemonWithEnoughCandy()
        {
            
            var blackList = new HashSet<PokemonId>();
            var pokemonToEvolve = await GetPokemonToEvolve();
            var pokemonSettings = await GetPokemonSettings();
            var allCandy = await GetCandy();

            foreach (var pokemon in pokemonToEvolve.Where(p => !_clientSettings.PokemonEvolutionFilter.Contains(p.PokemonId)))
            {
                if (blackList.Contains(pokemon.PokemonId)) continue;

                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                if (settings == null || settings.CandyToEvolve == 0) continue;

                var candy = allCandy.Where(c => c?.Item?.Count > settings?.CandyToEvolve && c?.Candy?.FamilyId == settings?.FamilyId).FirstOrDefault();
                if (candy == null) continue;

                var evolvePokemonOutProto = await _client.Inventory.EvolvePokemon(pokemon.Id);

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    Log.Write($"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExperienceAwarded}xp");
                }
                else
                {
                    Log.Write($"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}");
                    blackList.Add(pokemon.PokemonId);
                }
                await Randomization.RandomDelay(5000);
            }
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            await Randomization.RandomDelay(1000);
            var templates = await _client.Download.GetItemTemplates();
            var pokeSettings =  templates.ItemTemplates
                .Select(i => i.PokemonSettings)
                .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
            await Randomization.RandomDelay(1000);
            return pokeSettings;
        }

        public async Task<IEnumerable<InventoryItemData>> GetCandy()
        {
            var inventoryItems = await GetInventoryItems();
            var candy = inventoryItems
                .Where(i=>i.Candy !=null);
            await Randomization.RandomDelay(1000);
            return candy;
        }

        private async Task RecycleItems()
        {
            var items = await GetItemsToRecycle();

            foreach (var item in items)
            {
                await _client.Inventory.RecycleItem(item.ItemId, item.Count);
                Log.Write($"Recycled {item.Count}x {item.ItemId}");
                await Randomization.RandomDelay(1000);
            }
        }

        internal async Task TransferDuplicatePokemon()
        {
            var duplicatePokemons = await GetDuplicatePokemonToTransfer();

            foreach (var duplicatePokemon in duplicatePokemons)
            {
                await _client.Inventory.TransferPokemon(duplicatePokemon.Id);
                Log.Write($"Transfer {duplicatePokemon.PokemonId} with {duplicatePokemon.Cp} CP");
                await Randomization.RandomDelay(1000);
            }
        }

        private async Task<IEnumerable<InventoryItemData>> GetInventoryItems()
        {
            await Randomization.RandomDelay(1000);
            var inventory = await _client.Inventory.GetInventory();
            var inventoryItems = inventory.InventoryDelta.InventoryItems;
            await Randomization.RandomDelay(1000);
            return inventoryItems
                .Where(i=> i.InventoryItemData != null)
                .Select(i => i.InventoryItemData);
        }

        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer()
        {
            var inventoryItems = await GetInventoryItems();
            
            var pokemonList = inventoryItems
                .Select(i => i.PokemonData)
                .Where(p => p?.DeployedFortId != null && p.PokemonId > 0) 
                .ToList();

            return pokemonList
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Count() > _clientSettings.PokemonsToKeep)
                .SelectMany(p => p
                        .Where(x => x.Favorite == 0) // not starred
                        .OrderByDescending(x => x.Cp) // lowest CP
                        .ThenBy(n => n.StaminaMax) // lowest HP
                        .Skip(_clientSettings.PokemonsToKeep) // keep the desired amount
                        .ToList());
        }
        public async Task<IEnumerable<ItemData>> GetItemsToRecycle()
        {
            var allItems = await GetInventoryItems();

            return allItems
                .Where(x => x?.Item?.ItemId != null && x?.Item?.Count !=null)
                .Where(x => _clientSettings.ItemRecycleFilter.ContainsKey(x.Item.ItemId))
                .Where(x => x.Item.Count > _clientSettings.ItemRecycleFilter[x.Item.ItemId])
                .Select(x => new ItemData
                {
                    ItemId = x.Item.ItemId,
                    Count = x.Item.Count - _clientSettings.ItemRecycleFilter[x.Item.ItemId],
                    Unseen = x.Item.Unseen
                });
        }
        internal async Task<IEnumerable<FortData>> GetNearbyPokestops()
        {
            var mapObjects = await _client.Map.GetMapObjects();
            var pokeStops = mapObjects.MapCells
                .SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                .OrderBy(pokeStop => Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, pokeStop.Latitude, pokeStop.Longitude));
            return pokeStops;
        }

        private async Task<IEnumerable<MapPokemon>> GetNearbyPokemons()
        {
            var mapObjects = await _client.Map.GetMapObjects();

            var pokemons = mapObjects.MapCells
                .SelectMany(i => i.CatchablePokemons)
                .OrderBy(p => Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, p.Latitude, p.Longitude));
            return pokemons;
        }

        internal async Task<EncounterResponse> WalkToPokemon(MapPokemon pokemon)
        {
            var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, pokemon.Latitude, pokemon.Longitude);
            await Randomization.RandomDelayDistnace(distance, _clientSettings.SpeedKmh);
            await _client.Player.UpdatePlayerLocation(
                Randomization.GetRandomizedCoordinate(pokemon.Latitude), 
                Randomization.GetRandomizedCoordinate(pokemon.Longitude), 
                _clientSettings.DefaultAltitude);
            var encounter = await _client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
            return encounter;
        }

        private async Task CatchEncounter(EncounterResponse encounter, MapPokemon pokemon)
        {

            CatchPokemonResponse caughtPokemonResponse;
            do
            {
                var probability = encounter?.CaptureProbability?.CaptureProbability_;
                if (probability == null) return;

                if (probability.First() < 0.35)
                {
                    await _client.Encounter.UseCaptureItem(pokemon.EncounterId, ItemId.ItemRazzBerry, pokemon.SpawnPointId);
                    await Randomization.RandomDelay(2000);
                }

                var pokeball = await GetBestBall(encounter.WildPokemon);
                var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude,
                    pokemon.Latitude, pokemon.Longitude);
                caughtPokemonResponse = await _client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball);
                var success = caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess;
                Log.Write($" {(success ? '+' : '-')} {pokemon.PokemonId}: CP {encounter.WildPokemon?.PokemonData?.Cp}; probability: {encounter.CaptureProbability.CaptureProbability_.First()} [{pokeball}]");

                await Randomization.RandomDelay(2000);
            } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                     caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);
        }

        public  async Task WalkToPokeStop(FortData pokeStop)
        {
            var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, pokeStop.Latitude, pokeStop.Longitude);
            await Randomization.RandomDelayDistnace(distance, _clientSettings.SpeedKmh);
            await _client.Player.UpdatePlayerLocation(
                Randomization.GetRandomizedCoordinate(pokeStop.Latitude), 
                Randomization.GetRandomizedCoordinate(pokeStop.Longitude),
                _clientSettings.DefaultAltitude);
        }

        public  async Task FarmPokeStop(FortData pokeStop)
        {
            var fortInfo = await _client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
            var fortSearch = await _client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
            Log.Write($"Reached '{fortInfo.Name}'; XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}, Items: {StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded)}");
        }

        static readonly List<ItemId> _pokeballs = new List<ItemId>()
        {
            ItemId.ItemPokeBall,
            ItemId.ItemGreatBall,
            ItemId.ItemUltraBall,
            ItemId.ItemMasterBall
        };

        private async Task<ItemId> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;
           
            var inventoryItems = await GetInventoryItems();

            var balls = inventoryItems
                .Where (i=>i.Item != null)
                .Where (i=> _pokeballs.Contains(i.Item.ItemId) && i.Item?.Count > 0)
                .Select(i => i.Item)
                .ToDictionary(i => i.ItemId);
        
            if (pokemonCp > 1000)
                foreach (var ball in _pokeballs.Reverse<ItemId>())
                    if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;
  
            if (pokemonCp > 600)
                foreach (var ball in _pokeballs.Reverse<ItemId>().Skip(1))
                    if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            if (pokemonCp > 350)
                foreach (var ball in _pokeballs.Reverse<ItemId>().Skip(2))
                    if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            foreach (var ball in _pokeballs)
                if (balls.ContainsKey(ball) && balls[ball].Count > 0) return ball;

            Log.Write("We are out of pokeballs. Whoops.");
            throw new Exceptions.OutOfPokeBallsException();
        }

    }
}
