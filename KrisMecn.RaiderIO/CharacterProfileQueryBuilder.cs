using System.Collections.Generic;
using System.Threading.Tasks;
using KrisMecn.RaiderIO.Entities;
using KrisMecn.RaiderIO.Enums;

namespace KrisMecn.RaiderIO
{
    public class CharacterProfileQueryBuilder
    {
        private readonly RaiderIOSDK _sdk;
        private readonly Region _region;
        private readonly string _realm;
        private readonly string _characterName;

        private readonly List<string> _parts;

        public CharacterProfileQueryBuilder(RaiderIOSDK sdk, Region region, string realm, string characterName)
        {
            _sdk = sdk;
            _region = region;
            _realm = realm;
            _characterName = characterName;

            _parts = new List<string>();
        }

        public CharacterProfileQueryBuilder WithGear()
            => AddPart("gear");

        public CharacterProfileQueryBuilder WithGuild()
            => AddPart("guild");

        public CharacterProfileQueryBuilder WithRaidProgression()
            => AddPart("raid_progression");

        public CharacterProfileQueryBuilder WithMythicPlusRanks()
            => AddPart("mythic_plus_ranks");

        public CharacterProfileQueryBuilder WithMythicPlusRecentRuns()
            => AddPart("mythic_plus_recent_runs");

        public CharacterProfileQueryBuilder WithMythicPlusBestRuns()
            => AddPart("mythic_plus_best_runs");

        public CharacterProfileQueryBuilder WithMythicPlusHighestLevelRuns()
            => AddPart("mythic_plus_highest_level_runs");

        public CharacterProfileQueryBuilder WithMythicPlusWeeklyHighestLevelRuns()
            => AddPart("mythic_plus_weekly_highest_level_runs");

        public CharacterProfileQueryBuilder WithPreviousMythicPlusRanks()
            => AddPart("previous_mythic_plus_ranks");

        public CharacterProfileQueryBuilder WithRaidCurveInfo(params string[] raidSlugs)
            => AddPart($"raid_achievement_curve:{string.Join(":", raidSlugs)}");

        public CharacterProfileQueryBuilder WithMythicPlusScoresBySeason(params string[] seasonSlugs)
            => AddPart($"mythic_plus_scores_by_season:{string.Join(":", seasonSlugs)}");

        public string GetFields()
            => string.Join(",", _parts);

        public Task<CharacterProfile> Execute() {
            return _sdk.GetCharacterProfileRequest(
                _region,
                _realm,
                _characterName,
                GetFields()
            );
        }

        private CharacterProfileQueryBuilder AddPart(string part) {
            _parts.Add(part);
            return this;
        }
    }
}
