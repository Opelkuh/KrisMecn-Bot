using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace KrisMecn.Helpers.Extensions
{
    static class CommandContextExtensions
    {
        public static Task<DiscordMessage> ReplyToDM(this CommandContext ctx, string content = null, bool isTTS = false, DiscordEmbed embed = null)
        {
            if (ctx.Member == null)
                return ctx.RespondAsync(content, isTTS, embed);
            else
                return ctx.Member.SendMessageAsync(content, isTTS, embed);
        }
    }
}
