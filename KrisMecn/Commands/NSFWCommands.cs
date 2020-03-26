using KrisMecn.Extensions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace KrisMecn.Commands
{
    public class NSFWCommands : BaseKrisCommandModule
    {
        [
            Command("rule34"),
            Aliases("r34"),
            Description(@"TODO"),
            RequireNsfw()
        ]
        public Task Rule34(CommandContext ctx, [RemainingText] string tags) 
            => GetRandomImage(ctx, BooruSite.Rule34, tags);

        private async Task GetRandomImage(CommandContext ctx, BooruSite site, string tags)
        {
            var booru = ctx.Client.GetBooru();

            var img = await booru.GetRandomImage(BooruSite.Rule34, tags);

            var eb = new DiscordEmbedBuilder()
                .WithTitle("Image: ")
                .AddField("URL", img.fileUrl.AbsoluteUri)
                .AddField("Tags", tags)
                .WithImageUrl(img.fileUrl.AbsoluteUri)
                .WithFooter(ctx.Member.DisplayName, ctx.Member.AvatarUrl);

            await ctx.RespondAsync(embed: eb).ConfigureAwait(false);
        }
    }
}
