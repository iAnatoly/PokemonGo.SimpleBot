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
    using Exceptions;

    partial class Logic
    {
        private readonly Settings _clientSettings;
        private readonly Client _client;
        
        public Logic(Settings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
        }

        // TODO: split into multiple methods
        public async Task LoopWhileAuthIsValid()
        {
            int invalidResponseCount = 0;
            while (true)
            {
                try
                {
                    var visitedStops = new HashSet<string>();

                    while (true)
                    {
                        var player = await _client.Player.GetPlayer();
                        var mana = player.PlayerData.Currencies.Where(c => c?.Name == "STARDUST").FirstOrDefault();
                        Log.Write($" * You are {player.PlayerData.Username}; stardust: {mana.Amount}");
                        await Randomization.RandomDelay(10000);

                        if (_clientSettings.AllowTransfer) await TransferDuplicatePokemon();
                        if (_clientSettings.AllowEvolution) await EvolveAllPokemonWithEnoughCandy();
                        if (_clientSettings.AllowRecycle) await RecycleItems();

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
                            Log.Write("We are out of pokeballs. Let us go to the next pokestop.");
                        }
                    }
                }
                catch (InvalidResponseException ex)
                {
                    invalidResponseCount++;
                    
                    if (invalidResponseCount > 20)
                        throw new RepeatedInvalidResponseException();

                    Log.Write($"Exception: {ex.Message}; Cooling off for a minute", LogLevel.Warning);
                    await Randomization.RandomDelay(60000);
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
                catch (RepeatedInvalidResponseException)
                {
                    Log.Write("Number of invalid responses has reached a safety threshold; cooling off for 10 minutes", LogLevel.Error);
                    await Randomization.RandomDelay(600000);
                }
                await Randomization.RandomDelay(10000);
            }
        }
    }
}
 