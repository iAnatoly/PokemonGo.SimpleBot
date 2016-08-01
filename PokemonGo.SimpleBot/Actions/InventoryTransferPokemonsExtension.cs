using PokemonGo.Logger;
using PokemonGo.RocketAPI.Rpc;
using PokemonGo.SimpleBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.Actions
{
    static class InventoryTransferPokemonsExtension
    {
        public static async Task TransferDuplicatePokemon(this Inventory inventory, int pokemonsToKeep)
        {
            var inventoryItems = await inventory.GetInventoryItems();

            var duplicatePokemons = inventoryItems
                .Select(i => i.PokemonData)
                .Where(p => p?.DeployedFortId != null && p.PokemonId > 0) // not in agym, and actually a real pokemon
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Count() >pokemonsToKeep)
                .SelectMany(p => p
                        .Where(x => x.Favorite == 0) // not starred
                        .OrderByDescending(x => x.Cp) // lowest CP
                        .ThenBy(n => n.StaminaMax) // lowest HP
                        .Skip(pokemonsToKeep) // keep the desired amount
                        .ToList());
            

            foreach (var duplicatePokemon in duplicatePokemons)
            {
                await inventory.TransferPokemon(duplicatePokemon.Id);
                Log.Write($" > {duplicatePokemon.PokemonId} (CP {duplicatePokemon.Cp})");
                await Randomization.RandomDelay(1000);
            }

        }
    }
}
