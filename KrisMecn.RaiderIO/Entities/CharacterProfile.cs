using System;
using System.Collections.Generic;
using KrisMecn.RaiderIO.Enums;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class CharacterProfile
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("race")]
        public string Race;

        [JsonProperty("class")]
        public string Class;

        [JsonProperty("active_spec_name")]
        public string ActiveSpecName;

        [JsonProperty("active_spec_role")]
        public string ActiveSpecRole;

        [JsonProperty("gender")]
        public string Gender;

        [JsonProperty("faction")]
        public Faction Faction;

        [JsonProperty("achievement_points")]
        public int AchievementPoints = 0;

        [JsonProperty("honorable_kills")]
        public int HonorableKills = 0;

        [JsonProperty("thumbnail_url")]
        public Uri ThumbnailURL;

        [JsonProperty("region")]
        public Region Region;

        [JsonProperty("realm")]
        public string Realm;

        [JsonProperty("profile_url")]
        public Uri ProfileURL;

        [JsonProperty("gear")]
        public GearInfo? Gear;

        [JsonProperty("guild")]
        public GuildBasicInfo? Guild;

        [JsonProperty("raid_progression")]
        public Dictionary<string, RaidProgression>? RaidProgression;

        [JsonProperty("mythic_plus_scores_by_season")]
        public List<MythicPlusSeasonScore> MythicPlusSeasonScores;

        [JsonProperty("mythic_plus_ranks")]
        public MythicPlusRanks? MythicPlusRanks;

        [JsonProperty("previous_mythic_plus_ranks")]
        public MythicPlusRanks? PreviousMythicPlusRanks;

        [JsonProperty("mythic_plus_scores")]
        public MythicPlusScore? MythicPlusScores;

        [JsonProperty("previous_mythic_plus_scores")]
        public MythicPlusScore? PreviousMythicPlusScores;

        [JsonProperty("mythic_plus_recent_runs")]
        public List<MythicPlusRun>? MythicPlusRecentRuns;

        [JsonProperty("mythic_plus_best_runs")]
        public List<MythicPlusRun>? MythicPlusBestRuns;

        [JsonProperty("mythic_plus_highest_level_runs")]
        public List<MythicPlusRun>? MythicPlusHighestLevelRuns;

        [JsonProperty("mythic_plus_weekly_highest_level_runs")]
        public List<MythicPlusRun>? MythicPlusWeeklyHighestLevelRuns;

        [JsonProperty("mythic_plus_previous_weekly_highest_level_runs")]
        public List<MythicPlusRun>? MythicPlusPreviousWeeklyHighestLevelRuns;

        [JsonProperty("raid_achievement_curve")]
        public List<RaidCurveInfo>? RaidAchievementCurve;
    }
}
