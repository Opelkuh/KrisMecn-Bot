using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrisMecn.Helpers.Extensions;
using KrisMecn.RaiderIO;
using KrisMecn.RaiderIO.Enums;
using KrisMecn.RaiderIO.Entities;
using KrisMecn.RaiderIO.Exceptions;
using KrisMecn.Attributes;
using KrisMecn.RaiderIO.Slugs;

namespace KrisMecn.Commands
{
    [Group("wow")]
    [ModuleDisplayName("World of Warcraft Commands")]
    public class WoWCommands : BaseKrisCommandModule
    {
        // not the best idea but if it breaks it doesn't really matter
        private const string WOW_LOGO_URL = "https://i.imgur.com/PaYDCzi.png";
        private const string REGION_REMINDER = "Region can be US, EU, TW, KR or CN (default is EU)";

        private static Regex CHARACTER_REALM_REGEX = new Regex(@"([a-zA-Z]*)-(.*)");

        private readonly RaiderIOSDK _raiderIO;

        public WoWCommands()
        {
            _raiderIO = new RaiderIOSDK();
        }

        [Command("affixes")]
        public Task GetCurrentMythicPlusAffixes(CommandContext ctx)
            => GetCurrentMythicPlusAffixes(ctx, Region.EU);

        [
            Command("affixes"),
            Aliases("a", "af", "afixes", "affix", "afix"),
            Description("Returns currenty active Mythic+ affixes.\n" + REGION_REMINDER)
        ]
        public async Task GetCurrentMythicPlusAffixes(CommandContext ctx, Region region)
        {
            await ctx.TriggerTypingAsync();

            // fetch affixes from raider.io
            MythicPlusAffixes affixes;
            try
            {
                affixes = await _raiderIO.GetMythicPlusAffixes(region, Locale.EN);
            }
            catch (RaiderIOException exception)
            {
                await ctx.ReplyToDM(exception.Message);
                return;
            }

            if (affixes == null)
            {
                await ctx.ReplyToDM("Command failed. Try again later.");
                return;
            }

            // build embed response
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Active Mythic+ affixes")
                .WithUrl(affixes.LeaderboardURL)
                .WithAuthor("World of Warcraft", iconUrl: WOW_LOGO_URL);

            // add affix fields
            foreach (var affix in affixes.Affixes)
            {
                embed.AddField(affix.Name, affix.Description);
            }

            if (ctx.Member != null) embed.WithAuthorFooter(ctx.Member);

            // send embed
            await ctx.RespondAsync(embed: embed);
        }

        [Command("character")]
        public Task GetCharacter(CommandContext ctx, string characterNameWithRealm)
            => GetCharacter(ctx, characterNameWithRealm, Region.EU);

        [Command("character")]
        public Task GetCharacter(CommandContext ctx, string characterNameWithRealm, Region region)
        {
            var match = CHARACTER_REALM_REGEX.Match(characterNameWithRealm);
            foreach(Group group in match.Groups)
            {
                if(!group.Success || string.IsNullOrEmpty(group.Value))
                {
                    return ctx.ReplyToDM("Invalid input. Use (CHARACTER_NAME)-(REALM) format.");
                }
            }

            return GetCharacter(ctx, match.Groups[1].Value, match.Groups[2].Value, region);
        }

        [Command("character")]
        public Task GetCharacter(CommandContext ctx, string characterName, string realm)
            => GetCharacter(ctx, characterName, realm, Region.EU);

        [
            Command("character"),
            Aliases("c", "char", "profile", "p"),
            Description("Returns information about the target character.\n" + REGION_REMINDER)
        ]
        public async Task GetCharacter(CommandContext ctx, string characterName, string realm, Region region)
        {
            await ctx.TriggerTypingAsync();

            // fetch affixes from raider.io
            CharacterProfile profile;
            try
            {
                profile = await _raiderIO.GetCharacterProfile(region, realm, characterName)
                    .WithGear()
                    .WithGuild()
                    .WithMythicPlusScoresBySeason(SeasonSlug.Current)
                    .WithRaidProgression()
                    .WithRaidCurveInfo(RaidSlug.Latest)
                    .Execute();
            }
            catch (RaiderIOException exception)
            {
                await ctx.ReplyToDM(exception.Message);
                return;
            }

            if (profile == null)
            {
                await ctx.ReplyToDM("Command failed. Try again later.");
                return;
            }

            // remap curve data into a dictionary
            var curveInfoMap = new Dictionary<string, RaidCurveInfo>();
            if (profile.RaidAchievementCurve != null)
            {
                foreach (var curveInfo in profile.RaidAchievementCurve)
                {
                    curveInfoMap.Add(curveInfo.RaidSlug, curveInfo);
                }
            }

            // build embed response
            var embed = new DiscordEmbedBuilder()
                .WithAuthor("World of Warcraft", iconUrl: WOW_LOGO_URL)
                .WithTitle($"{profile.Name}-{profile.Realm}")
                .WithUrl(profile.ProfileURL)
                .WithThumbnailUrl(profile.ThumbnailURL)
                .AddField("CHARACTER INFO", @"══════════════════")
                .AddField("Race", $"{profile.Gender.ToFirstCharUppercase()} {profile.Race}")
                .AddField("Class", profile.Class, true)
                .AddField("Spec", $"{profile.ActiveSpecRole} - {profile.ActiveSpecName}", true)
                .AddField("Item Level", profile.Gear.ItemLevelEquipped.ToString())
                .AddField("Achivement Points", profile.AchievementPoints.ToString(), true);

            // add m+ score if available
            if (profile.MythicPlusSeasonScores != null && profile.MythicPlusSeasonScores.Count > 0)
            {
                var mPlusScore = profile.MythicPlusSeasonScores[0].Score;
                embed
                    .AddField("RAIDER.IO SCORE", @"══════════════════")
                    .AddField(":star: Total", mPlusScore.All.ToString());

                if (mPlusScore.DPS > 0) embed.AddField("DPS", mPlusScore.DPS.ToString(), true);
                if (mPlusScore.Tank > 0) embed.AddField("Tank", mPlusScore.Tank.ToString(), true);
                if (mPlusScore.Healer > 0) embed.AddField("Healer", mPlusScore.Healer.ToString(), true);
            }

            // add raid progress
            if (profile.RaidProgression != null && profile.RaidProgression.TryGetValue(RaidSlug.Latest, out var raidProg))
            {
                embed
                    .AddField("RAID PROGRESS", @"══════════════════");

                var totBoss = raidProg.TotalBosses;
                var progressBuilder = new StringBuilder();
                if (raidProg.MythicBossesKilled > 0) progressBuilder.AppendLine(FormatRaidProgress(raidProg.MythicBossesKilled, totBoss, "Mythic"));
                if (raidProg.HeroicBossesKilled > 0) progressBuilder.AppendLine(FormatRaidProgress(raidProg.HeroicBossesKilled, totBoss, "Heroic"));
                if (raidProg.NormalBossesKilled > 0) progressBuilder.AppendLine(FormatRaidProgress(raidProg.NormalBossesKilled, totBoss, "Normal"));

                // no kills fallback
                if (progressBuilder.Length <= 0) progressBuilder.Append("Nothing");

                // get raid display name or fallback to generic title
                string raidDisplayName = "Boss kills";
                RaidSlug.DisplayNames.TryGetValue(RaidSlug.Latest, out raidDisplayName);

                // get curve info
                var hasCurve = "No";
                if (curveInfoMap.TryGetValue(RaidSlug.Latest, out var curveInfo))
                {
                    var dateFormat = "dd.MM.yyyy";
                    if (curveInfo.CuttingEdgeRecieved.HasValue) hasCurve = $"Cutting Edge ({curveInfo.CuttingEdgeRecieved.Value.ToString(dateFormat)})";
                    else if (curveInfo.AheadOfTheCurveRecieved.HasValue) hasCurve = $"Yes ({curveInfo.CuttingEdgeRecieved.Value.ToString(dateFormat)})";
                }

                embed
                    .AddField(raidDisplayName, progressBuilder.ToString(), true)
                    .AddField("Has curve?", hasCurve, true);
            }

            if (ctx.Member != null) embed.WithAuthorFooter(ctx.Member);

            // send embed
            await ctx.RespondAsync(embed: embed);
        }

        private string FormatRaidProgress(int kills, int totalBosses, string difficulty)
        {
            return $"{kills}/{totalBosses} {difficulty}";
        }
    }
}
