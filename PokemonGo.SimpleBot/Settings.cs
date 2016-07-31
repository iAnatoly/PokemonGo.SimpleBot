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
        public bool AllowFarming=> UserSettings.Default.AllowFarming;

        public int MaxPokemonsPerPokestop => UserSettings.Default.MaxPOkemonsPerPokestop;
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

        public Dictionary<ItemId, int> ItemRecycleFilter => new Dictionary<ItemId, int> 
        {

            { ItemId.ItemUnknown, 0 },
            { ItemId.ItemPokeBall, 50 },
            { ItemId.ItemGreatBall, 50 },
            { ItemId.ItemUltraBall, 50 },
            { ItemId.ItemMasterBall, 100 },

            { ItemId.ItemPotion, 0 },
            { ItemId.ItemSuperPotion, 0 },
            { ItemId.ItemHyperPotion, 50 },
            { ItemId.ItemMaxPotion, 50 },

            { ItemId.ItemRevive, 10 },
            { ItemId.ItemMaxRevive, 50 },

            { ItemId.ItemLuckyEgg, 200},

            { ItemId.ItemIncenseOrdinary, 100 },
            { ItemId.ItemIncenseSpicy, 100 },
            { ItemId.ItemIncenseCool, 100 },
            {ItemId.ItemIncenseFloral, 100 },

            {ItemId.ItemTroyDisk, 100 },
            {ItemId.ItemXAttack, 100 },
            {ItemId.ItemXDefense, 100 },
            {ItemId.ItemXMiracle, 100 },

            {ItemId.ItemRazzBerry, 20 },
            {ItemId.ItemBlukBerry, 10 },
            {ItemId.ItemNanabBerry, 10 },
            {ItemId.ItemWeparBerry, 30 },
            {ItemId.ItemPinapBerry, 30 },

            {ItemId.ItemSpecialCamera, 100 },
            {ItemId.ItemIncubatorBasicUnlimited, 100 },
            {ItemId.ItemIncubatorBasic, 100 },
            {ItemId.ItemPokemonStorageUpgrade, 100 },
            {ItemId.ItemItemStorageUpgrade, 100 },
        };

        public string GoogleRefreshToken { get; set; }

        public double DefaultLatitude
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

        public double DefaultLongitude
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

        public double DefaultAltitude
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
