using POGOProtos.Inventory.Item;
using PokemonGo.Logger;
using PokemonGo.RocketAPI.Rpc;
using PokemonGo.SimpleBot.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.Actions
{
    static class RecycleItemsExt
    {
        public static async Task RecycleItems(this Inventory inventory, IDictionary<ItemId, int> itemRecycleFilter)
        {
            var allItems = await inventory.GetInventoryItems();

            var items = allItems
                .Where(x => x?.Item?.ItemId != null && x?.Item?.Count != null)
                .Where(x => itemRecycleFilter.ContainsKey(x.Item.ItemId))
                .Where(x => x.Item.Count > itemRecycleFilter[x.Item.ItemId])
                .Select(x => new ItemData
                {
                    ItemId = x.Item.ItemId,
                    Count = x.Item.Count - itemRecycleFilter[x.Item.ItemId],
                    Unseen = x.Item.Unseen
                });

            foreach (var item in items)
            {
                await inventory.RecycleItem(item.ItemId, item.Count);
                Log.Write($" > {item.Count}x {item.ItemId}");
                await Randomization.RandomDelay(1000);
            }
        }
    }
}