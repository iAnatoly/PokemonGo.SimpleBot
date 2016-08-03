using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.Logger;


namespace PokemonGo.SimpleBot
{
    using RocketAPI.Exceptions;
    using Utils;
    using Exceptions;
    using Actions;

    partial class Logic
    {
        private readonly Settings _clientSettings;
        private readonly Client _client;
        private readonly Evolution _evolution;
        private readonly Hunting _hunting;
        private readonly Farming _farming;
        
        public Logic(Settings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings, new APIFailureStrategy());

            _evolution = new Evolution(_client, _clientSettings);
            _hunting = new Hunting(_client, _clientSettings);
            _farming = new Farming(_client, _clientSettings);

        }

        private async Task PrintPlayerStats()
        {
            var player = await _client.Player.GetPlayer();
            var mana = player.PlayerData.Currencies.FirstOrDefault(c => c?.Name == "STARDUST");
            Log.Write($"You are {player.PlayerData.Username}; stardust: {mana?.Amount}");
            await Randomization.RandomDelay(10000);
        }

        private async Task LoopWhileAuthIsValid()
        {
            while (true)
            {
                try
                {
                    var visitedStops = new HashSet<string>();

                    while (true)
                    {
                        await PrintPlayerStats();

                        if (_clientSettings.AllowTransfer) await _client.Inventory.TransferDuplicatePokemon(_clientSettings.PokemonsToKeep);
                        if (_clientSettings.AllowEvolution) await _evolution.EvolveAllPokemonWithEnoughCandy();
                        if (_clientSettings.AllowRecycle) await _client.Inventory.RecycleItems(_clientSettings.ItemRecycleFilter);

                        var pokestops = await _farming.GetNearbyPokestops();

                        var nearestUnseenStop = pokestops.FirstOrDefault(pokestop => !visitedStops.Contains(pokestop.Id));

                        if (nearestUnseenStop == null) break;

                        await _farming.WalkToPokeStop(nearestUnseenStop);
                        visitedStops.Add(nearestUnseenStop.Id);

                        if (_clientSettings.AllowFarming) await _farming.FarmPokeStop(nearestUnseenStop);

                        try
                        {
                            if (_clientSettings.AllowCatching) await HuntPokemonsLoop();
                        }
                        catch (OutOfPokeBallsException)
                        {
                            Log.Write(" ! We are out of pokeballs. Let us go to the next pokestop.");
                        }
                    }
                }
                catch (InvalidResponseException ex)
                {
                    ErrorStats.RegisterException(ex);

                    if (ErrorStats.GetNumberOfRecentErrors(ex, 10)>3)
                        throw new RepeatedInvalidResponseException();

                    Log.Write($"Exception: {ex.Message} Cooling off for a minute", LogLevel.Warning);
                    await Randomization.RandomDelay(60*1000);
                }
                catch (Exception ex)
                {
                    Log.Write($"Exception: {ex}", LogLevel.Error);
                }
            }
        }

        private async Task HuntPokemonsLoop()
        {
            var visitedPokemons = new HashSet<ulong>();

            for (var sightings = 0; sightings < _clientSettings.MaxPokemonsPerPokestop; sightings++)
            {
                var nearbyPokemons = await _hunting.GetNearbyPokemons();

                var nearestUnseenPokemon = nearbyPokemons.FirstOrDefault(p => !visitedPokemons.Contains(p.EncounterId));
                if (nearestUnseenPokemon == null) break;

                var encounter = await _hunting.WalkToPokemon(nearestUnseenPokemon);
                visitedPokemons.Add(nearestUnseenPokemon.EncounterId);
                await _hunting.CatchEncounter(encounter, nearestUnseenPokemon);
            }
        }

        public async Task Execute()
        {
            while (true)
            {
                try
                {
                    await _client.Login.DoLogin();
                    await LoopWhileAuthIsValid();
                }
                catch (AccessTokenExpiredException)
                {
                    Log.Write($"Access token expired");
                    await Randomization.RandomDelay(10000);
                }
                catch (TaskCanceledException)
                {
                    Log.Write($"Tasks are being cancelled - server unresponsive. Cooling off and restarting.");
                    await Randomization.RandomDelay(60 * 10 * 1000);
                }
                catch (RepeatedInvalidResponseException ex)
                {
                    ErrorStats.RegisterException(ex);
                    if (ErrorStats.GetNumberOfRecentErrors(ex, 60) > 2)
                    {
                        Log.Write("Too many errors per hour; terminating", LogLevel.Error);
                        return;
                    }
                    else
                    {
                        Log.Write("Number of invalid responses has reached a safety threshold; cooling off for 5-10 minutes", LogLevel.Error);
                        await Randomization.RandomDelay(60 * 10 * 1000);
                    }
                }
            }
        }
    }
}
 