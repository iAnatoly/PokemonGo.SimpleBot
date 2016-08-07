using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Data.Player;

namespace PokemonGo.SimpleBot.Extensions
{
    static class GetPlayerStatsExt
    {
        public static async Task<PlayerStats> GetPlayerStats(this Inventory inventory)
        {
            var items = await inventory.GetInventoryItems();
            var stat = items.Select(item => item.PlayerStats).FirstOrDefault(p => p != null);
            return stat;
        }
    }
}
