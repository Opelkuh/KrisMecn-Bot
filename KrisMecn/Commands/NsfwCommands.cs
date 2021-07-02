using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using KrisMecn.Extensions;
using KrisMecn.Helpers.Extensions;
using System.Text;
using System.Threading.Tasks;

namespace KrisMecn.Commands
{
    public class NsfwCommands : BaseKrisCommandModule
    {
        private static DiscordColor _r34Color = new DiscordColor("#AAE5A3");
        private static DiscordColor _danbooruColor = new DiscordColor("#0073FF");
        private static DiscordColor _gelbooruColor = new DiscordColor("#005CD0");
        private static DiscordColor _sankakuComplexColor = new DiscordColor("#FF761D");
        private static DiscordColor _safebooruColor = new DiscordColor("#F0FFFF");

        [
            Command("rule34"),
            Aliases("r34"),
            Description("Gets a random image from rule34.xxx with the specified tags"),
            RequireNsfw()
        ]
        public Task Rule34(CommandContext ctx, [RemainingText] string tags)
            => GetRandomImage(ctx, BooruSite.Rule34, tags, _r34Color);

        [
            Command("danbooru"),
            Aliases("db"),
            Description("Gets a random image from danbooru.donmai.us with the specified tags"),
            RequireNsfw()
        ]
        public Task Danbooru(CommandContext ctx, [RemainingText] string tags)
            => GetRandomImage(ctx, BooruSite.Danbooru, tags, _danbooruColor);

        [
            Command("gelbooru"),
            Aliases("gb"),
            Description("Gets a random image from gelbooru.com with the specified tags"),
            RequireNsfw()
        ]
        public Task Gelbooru(CommandContext ctx, [RemainingText] string tags)
            => GetRandomImage(ctx, BooruSite.Gelbooru, tags, _gelbooruColor);

        [
            Command("sankakucomplex"),
            Aliases("sankaku"),
            Description("Gets a random image from beta.sankakucomplex.com with the specified tags"),
            RequireNsfw()
        ]
        public Task SankakuComplex(CommandContext ctx, [RemainingText] string tags)
            => GetRandomImage(ctx, BooruSite.SankakuComplex, tags, _sankakuComplexColor);

        [
            Command("safebooru"),
            Aliases("sb"),
            Description("Gets a random image from safebooru.org with the specified tags")
        ]
        public Task Safebooru(CommandContext ctx, [RemainingText] string tags)
            => GetRandomImage(ctx, BooruSite.Safebooru, tags, _safebooruColor);

        private async Task GetRandomImage(CommandContext ctx, BooruSite site, string tags, DiscordColor? color = null)
        {
            await ctx.TriggerTypingAsync();

            var booru = ctx.Client.GetBooru();

            // get string site name
            string siteName = booru.GetSiteName(site);

            // prepare embed builder
            var eb = new DiscordEmbedBuilder()
                .WithTitle($"Image from {siteName} with tags `{tags}`: ");

            // add color and author info
            if (color.HasValue) eb.WithColor(color.Value);
            if (ctx.Member != null) eb.WithAuthorFooter(ctx.Member, "Requested by: "); // `Member` is null in DM channels

            // fetch image
            var response = await booru.GetRandomImage(site, tags);

            // don't continue if no image was found
            if (!response.HasValue)
            {
                eb.WithDescription("No image found");
                await ctx.RespondAsync(embed: eb.Build());
                return;
            }

            var img = response.Value;

            // build response embed
            eb
                .AddField("URL", img.fileUrl.AbsoluteUri)
                .AddField("Tags", JoinTags(img.tags))
                .WithImageUrl(img.fileUrl.AbsoluteUri);

            // add optional fields
            if (img.score.HasValue) eb.AddField("Likes", img.score.Value.ToString(), true);
            if (img.creation.HasValue) eb.AddField("Upload date (D.M.Y)", img.creation.Value.ToString("dd.MM.yyyy"), true);

            await ctx.RespondAsync(embed: eb).ConfigureAwait(false);
        }

        private string JoinTags(string[] data)
        {
            var sb = new StringBuilder();

            foreach (string s in data)
            {
                sb.AppendFormat("`{0}` ", s);
            }

            return sb.ToString();
        }
    }
}
