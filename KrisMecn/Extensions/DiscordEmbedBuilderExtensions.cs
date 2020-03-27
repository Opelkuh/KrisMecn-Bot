using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KrisMecn.Extensions
{
    static class DiscordEmbedBuilderExtensions
    {
        public static DiscordEmbedBuilder WithAuthor(this DiscordEmbedBuilder eb, DiscordMember member)
            => eb.WithAuthor(member.DisplayName, iconUrl: member.AvatarUrl);

        public static DiscordEmbedBuilder WithAuthorFooter(this DiscordEmbedBuilder eb, DiscordMember member, string prefix = "")
            => eb.WithFooter($"{prefix}{member.DisplayName}", member.AvatarUrl);
    }
}
