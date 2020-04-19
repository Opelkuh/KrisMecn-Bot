using DSharpPlus.Entities;

namespace KrisMecn.Helpers.Extensions
{
    static class DiscordUserExtensions
    {
        public static string GetFullName(this DiscordUser user)
            => $"{user.Username}#{user.Discriminator}";
    }
}
