using DSharpPlus.Entities;

namespace KrisMecn.Helpers.Extensions
{
    static class DiscordEmbedBuilderExtensions
    {
        public static DiscordEmbedBuilder WithAuthor(this DiscordEmbedBuilder eb, DiscordMember member)
            => eb.WithAuthor(member.DisplayName, iconUrl: member.AvatarUrl);

        public static DiscordEmbedBuilder WithAuthorFooter(this DiscordEmbedBuilder eb, DiscordMember member, string prefix = "Requested by: ")
            => eb.WithFooter($"{prefix}{member.DisplayName}", member.AvatarUrl);
    }
}
