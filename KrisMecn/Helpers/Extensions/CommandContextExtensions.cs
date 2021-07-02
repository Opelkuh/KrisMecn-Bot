using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace KrisMecn.Helpers.Extensions
{
    static class CommandContextExtensions
    {
        public static Task<DiscordMessage> ReplyToDM(this CommandContext ctx, string content = null, DiscordEmbed embed = null)
        {
            if (ctx.Member == null)
            {
                if (embed is null) return ctx.RespondAsync(content);
                else return ctx.RespondAsync(content, embed);
            }
            else
            {
                if (embed is null) return ctx.Member.SendMessageAsync(content);
                else return ctx.Member.SendMessageAsync(content, embed);
            }
        }
    }
}
