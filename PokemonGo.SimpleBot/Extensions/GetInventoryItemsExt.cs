﻿using POGOProtos.Inventory;
using PokemonGo.RocketAPI.Rpc;
using PokemonGo.SimpleBot.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.Extensions
{
    static class GetInventoryItemsExt
    {
        public static async Task<IEnumerable<InventoryItemData>> GetInventoryItems(this Inventory inv)
        {
            await Randomization.RandomDelay(1000);
            var inventory = await inv.GetInventory();
            var inventoryItems = inventory.InventoryDelta.InventoryItems;
            await Randomization.RandomDelay(1000);
            return inventoryItems
                .Where(i => i.InventoryItemData != null)
                .Select(i => i.InventoryItemData)
                .ToList();
        }
    }
}
