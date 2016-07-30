using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using POGOProtos.Enums;

namespace PokemonGo.SimpleBot
{
    public class Settings : ISettings
    {
        // unused pramaters
        public AuthType AuthType { get; set; }
        public string PtcUsername { get; set; }
        public string PtcPassword { get; set; }
       

        public int MaxPokestops => 1000;
        public double SpeedKmh => UserSettings.Default.WalkingSpeedKmh;

        public bool AllowEvolution => UserSettings.Default.AllowEvolution;
        public bool AllowRecycle => UserSettings.Default.AllowRecycle;
        public bool AllowTransfer => UserSettings.Default.AllowTransfer;
        public bool AllowCatching => UserSettings.Default.AllowCatching;

        public int PokemonsToKeep => UserSettings.Default.NumberOfSamePokemonsToKeep;

        public string GoogleUsername { get; set; }
        public string GooglePassword { get; set; }

        public HashSet<PokemonId> PokemonEvolutionFilter { get; } = new HashSet<PokemonId>()
        {
            // can evolve 3 levels
            PokemonId.Dratini, PokemonId.Dragonair,
            PokemonId.Geodude, PokemonId.Graveler, 
            PokemonId.Abra, PokemonId.Kadabra,
            PokemonId.Charizard, PokemonId.Charmeleon,
            PokemonId.Poliwag, PokemonId.Poliwhirl,
            PokemonId.Machop, PokemonId.Machoke,

            //rare candy:
            PokemonId.Magikarp,
            PokemonId.Onix,
            PokemonId.Exeggcute,
            PokemonId.Rhyhorn,

            // can evolve into different species:
            PokemonId.Eevee, 
        };

        IEnumerable<KeyValuePair<ItemId, int>> ItemRecycleFilter => new[]
        {
            new KeyValuePair<ItemId, int>(ItemId.ItemUnknown, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, 50), 
            new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, 100),

            new KeyValuePair<ItemId, int>(ItemId.ItemPotion, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, 50),

            new KeyValuePair<ItemId, int>(ItemId.ItemRevive, 10),
            new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, 50),

            new KeyValuePair<ItemId, int>(ItemId.ItemLuckyEgg, 200),

            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseOrdinary, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseSpicy, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseCool, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseFloral, 100),

            new KeyValuePair<ItemId, int>(ItemId.ItemTroyDisk, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXAttack, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXDefense, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXMiracle, 100),

            new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, 20),
            new KeyValuePair<ItemId, int>(ItemId.ItemBlukBerry, 10),
            new KeyValuePair<ItemId, int>(ItemId.ItemNanabBerry, 10),
            new KeyValuePair<ItemId, int>(ItemId.ItemWeparBerry, 30),
            new KeyValuePair<ItemId, int>(ItemId.ItemPinapBerry, 30),

            new KeyValuePair<ItemId, int>(ItemId.ItemSpecialCamera, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasicUnlimited, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasic, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemPokemonStorageUpgrade, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemItemStorageUpgrade, 100),
        };

        public string GoogleRefreshToken { get; set; }

        double ISettings.DefaultLatitude
        {
            get
            {
                return UserSettings.Default.Latitude;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        double ISettings.DefaultLongitude
        {
            get
            {
                return UserSettings.Default.Longitude;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        double ISettings.DefaultAltitude
        {
            get
            {
                return UserSettings.Default.Altitude;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
