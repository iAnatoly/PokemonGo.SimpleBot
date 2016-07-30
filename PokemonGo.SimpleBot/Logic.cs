using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.Logger;


namespace PokemonGo.SimpleBot
{
    using Utils;

    class Logic
    {
        private readonly Settings _clientSettings;
        private readonly Client _client;
        
        public Logic(Settings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
        }

        public async Task Execute()
        {
            while (true)
            {
                await _client.Login.DoGoogleLogin(_clientSettings.GoogleUsername, _clientSettings.GooglePassword);

                //var inventory = await _client.Inventory.GetInventory();
                var player = await _client.Player.GetPlayer();
                Log.Write($"You are {player.PlayerData.Username}");

                
                /*var settings = await _client.Download.GetSettings();
                var mapObjects = await _client.Map.GetMapObjects();
                var updateLocation = await _client.Player.UpdatePlayerLocation();
                var encounter = await _client.Encounter.EncounterPokemon(encId, spawnId);
                var catchPokemon = await _client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball)
                var evolvePokemon = await _client.Inventory.EvolvePokemon(pokemonId);
                var transfer = await _client.Inventory.TransferPokemon(pokemonId);
                var recycle = await _client.Inventory.RecycleItem(item.ItemId, item.Count);
                var useBerry = await _client.Encounter.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
                var fortInfo = await _client.Fort.GetFort(pokeStopId, pokeStopLatitude, pokeStopLongitude);
                var fortSearch = await _client.Fort.SearchFort(pokeStopId, pokeStopLatitude, pokeStopLongitude);
                */
                await Randomization.RandomDelay(10000);
            }
        }
    }
}
