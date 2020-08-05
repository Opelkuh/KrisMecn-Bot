using System.Collections.Generic;

namespace KrisMecn.RaiderIO.Slugs
{
    public class RaidSlug
    {
        /// <summary>
        /// Specifies the slug of the latest raid
        /// </summary>
        public const string Latest = NyalothaTheWakingCity;

        public const string NyalothaTheWakingCity = "nyalotha-the-waking-city";
        public const string TheEternalPalace = "the-eternal-palace";
        public const string CrucibleOfStorms = "crucible-of-storms";
        public const string BattleOfDazaralor = "battle-of-dazaralor";
        public const string Uldir = "uldir";
        public const string AntorusTheBurningThrone = "antorus-the-burning-throne";
        public const string TombOfSargeras = "tomb-of-sargeras";
        public const string TheNigthold = "the-nighthold";
        public const string TrialOfValor = "trial-of-valor";
        public const string TheEmeraldNightmare = "the-emerald-nightmare";

        public static Dictionary<string, string> DisplayNames = new Dictionary<string, string>()
        {
            { NyalothaTheWakingCity, "Ny'alotha, The Waking City" },
            { TheEternalPalace, "The Eternal Palace" },
            { CrucibleOfStorms, "Crucible of Storms" },
            { BattleOfDazaralor, "Battle of Dazar'alor" },
            { Uldir, "Uldir" },
            { AntorusTheBurningThrone, "Antorus, the Burning Throne" },
            { TombOfSargeras, "Tomb of Sargeras" },
            { TheNigthold, "The Nighthold" },
            { TrialOfValor, "Trial of Valor" },
            { TheEmeraldNightmare, "The Emerald Nightmare" },
        };
    }
}
