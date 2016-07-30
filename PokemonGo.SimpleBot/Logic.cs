using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.Logger;


namespace PokemonGo.SimpleBot
{
    using RocketAPI.Exceptions;
    using Utils;

    partial class Logic
    {
        private readonly Settings _clientSettings;
        private readonly Client _client;
        
        public Logic(Settings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
        }

        public async Task LoopWhileAuthIsValid()
        {
            while (true)
            {
                try
                {
                    var player = await _client.Player.GetPlayer();
                    var mana = player.PlayerData.Currencies.Where(c => c?.Name == "STARDUST").FirstOrDefault();
                    
                    Log.Write($"You are {player.PlayerData.Username}; stardust: {mana.Amount}");

                    if (_clientSettings.AllowTransfer) await TransferDuplicatePokemon();
                    if (_clientSettings.AllowEvolution) await EvolveAllPokemonWithEnoughCandy();
                    if (_clientSettings.AllowRecycle) await RecycleItems();

                    var visitedStops = new HashSet<string>();

                    while (true)
                    {
                        var pokestops = await GetNearbyPokestops();

                        var nearestUnseenStop = pokestops
                            .Where(pokestop => !visitedStops.Contains(pokestop.Id))
                            .FirstOrDefault();

                        if (nearestUnseenStop == null) break;

                        await WalkToPokeStop(nearestUnseenStop);
                        visitedStops.Add(nearestUnseenStop.Id);

                        if (_clientSettings.AllowFarming) await FarmPokeStop(nearestUnseenStop);

                        try
                        {
                            if (_clientSettings.AllowCatching)
                            {
                                var visitedPokemons = new HashSet<ulong>();

                                for (int sightings = 0; sightings < _clientSettings.MaxPokemonsPerPokestop; sightings++)
                                {
                                    var nearbyPokemons = await GetNearbyPokemons();

                                    var nearestUnseenPokemon = nearbyPokemons
                                        .Where(p => !visitedPokemons.Contains(p.EncounterId))
                                        .FirstOrDefault();

                                    if (nearestUnseenPokemon == null) break;

                                    var encounter = await WalkToPokemon(nearestUnseenPokemon);
                                    visitedPokemons.Add(nearestUnseenPokemon.EncounterId);
                                    await CatchEncounter(encounter, nearestUnseenPokemon);
                                }
                            }
                        }
                        catch (OutOfPokeBallsException)
                        {
                            Log.Write("We are out of pokeballs. Let us go to next pokestop");
                        }
                    }

                    /*
                    var updateLocation = await _client.Player.UpdatePlayerLocation();
                    var settings = await _client.Download.GetSettings();
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
                }
                catch (InvalidResponseException ex)
                {
                    Log.Write($"Exception: {ex.Message}", LogLevel.Error);
                }
                catch (Exception ex)
                {
                    Log.Write($"Exception: {ex}", LogLevel.Error);
                }
            }
        }

        public async Task Execute()
        {
            while (true)
            {
                try
                {
                    await _client.Login.DoGoogleLogin(_clientSettings.GoogleUsername, _clientSettings.GooglePassword);
                    await LoopWhileAuthIsValid();
                }
                catch (AccessTokenExpiredException)
                {
                    Log.Write($"Access token expired");
                }
                await Randomization.RandomDelay(10000);
            }
        }
    }
}
 