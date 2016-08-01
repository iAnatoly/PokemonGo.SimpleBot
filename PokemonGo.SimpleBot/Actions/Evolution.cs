using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using PokemonGo.Logger;
using PokemonGo.RocketAPI;
using PokemonGo.SimpleBot.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.Actions
{
    class Evolution
    {
        private readonly Client _client;
        private readonly Settings _clientSettings;

        public Evolution(Client client, Settings settings)
        {
            _client = client;
            _clientSettings = settings;
        }

        private static IEnumerable<PokemonData> GetPokemonToEvolve(IEnumerable<InventoryItemData> inventoryItems)
        {
            var playerPokemons = inventoryItems
                .Select(i => i.PokemonData)
                .Where(p => p != null && p.PokemonId > 0);
            var pokemons = playerPokemons
                .Where(p => p.DeployedFortId != null) //Don't evolve pokemon in gyms
                .OrderByDescending(p => p.Cp) // evolve highest CP first, do not waste candy on XP
                .ToList();
            return pokemons;
        }

        private static IEnumerable<InventoryItemData> GetCandy(IEnumerable<InventoryItemData> inventoryItems)
        {
            var candy = inventoryItems
                .Where(i => i.Candy != null && i.Candy.Candy_ > 0)
                .ToList();
            return candy;
        }

        internal async Task EvolveAllPokemonWithEnoughCandy()
        {
            var inventory = await _client.Inventory.GetInventoryItems();
            var blackList = new HashSet<PokemonId>();
            var pokemonToEvolve = GetPokemonToEvolve(inventory);
            var pokemonCandy = GetCandy(inventory);
            var pokemonSettings = await GetPokemonSettings();
            

            foreach (var pokemon in pokemonToEvolve.Where(p => !_clientSettings.PokemonEvolutionFilter.Contains(p.PokemonId)))
            {
                if (blackList.Contains(pokemon.PokemonId)) continue;

                var settings = pokemonSettings
                    .FirstOrDefault(x => x.PokemonId == pokemon.PokemonId);
                if (settings == null || settings.CandyToEvolve == 0) continue;

                var candy = pokemonCandy
                    .FirstOrDefault(c => c.Candy.FamilyId == settings.FamilyId && c.Candy.Candy_ >= settings.CandyToEvolve);
                if (candy == null) continue;
                
                var evolveResponse = await _client.Inventory.EvolvePokemon(pokemon.Id);

                if (evolveResponse.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    Log.Write($" + Evolved {pokemon.PokemonId} for {evolveResponse.ExperienceAwarded}xp");
                }
                else
                {
                    Log.Write($" - Failed to evolve {pokemon.PokemonId}; Reason: {evolveResponse.Result}");
                    blackList.Add(pokemon.PokemonId);
                }
                await Randomization.RandomDelay(5000);
            }
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            await Randomization.RandomDelay(1000);
            var templates = await _client.Download.GetItemTemplates();
            var pokeSettings = templates.ItemTemplates
                .Select(i => i.PokemonSettings)
                .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset)
                .ToList();
            await Randomization.RandomDelay(1000);
            return pokeSettings;
        }
        
    }
}
