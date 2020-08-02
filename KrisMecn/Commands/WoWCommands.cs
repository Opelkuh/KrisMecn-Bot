using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KrisMecn.Helpers.Extensions;
using KrisMecn.RaiderIO;
using KrisMecn.RaiderIO.Enums;
using KrisMecn.RaiderIO.Entities;
using KrisMecn.RaiderIO.Exceptions;
using KrisMecn.Attributes;

namespace KrisMecn.Commands
{
    [Group("wow")]
    [ModuleDisplayName("World of Warcraft Commands")]
    public class WoWCommands : BaseKrisCommandModule
    {
        // not the best idea but if it breaks it doesn't really matter
        private const string WOW_LOGO_URL = "https://i.imgur.com/PaYDCzi.png";

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
            Description("Returns currenty active Mythic+ affixes.\nRegion can be US, EU, TW, KR or CN (default is EU)")
        ]
        public async Task GetCurrentMythicPlusAffixes(CommandContext ctx, Region region)
        {
            await ctx.TriggerTypingAsync();

            // fetch affixes from raider.io
            MythicPlusAffixes affixes;
            try {
                affixes = await _raiderIO.GetMythicPlusAffixes(region, Locale.EN);
            } catch(RaiderIOException exception) {
                await ctx.ReplyToDM(exception.Message);
                return;
            }

            if(affixes == null) {
                await ctx.ReplyToDM("Command failed. Try again later.");
                return;
            }

            // build embed response
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Active Mythic+ affixes")
                .WithUrl(affixes.LeaderboardURL)
                .WithAuthor("World of Warcraft", iconUrl: WOW_LOGO_URL);

            // add affix fields
            foreach(var affix in affixes.Affixes) {
                embed.AddField(affix.Name, affix.Description);
            }
            
            if(ctx.Member != null) embed.WithAuthorFooter(ctx.Member);

            // send embed
            await ctx.RespondAsync(embed: embed);
        }
    }
}
