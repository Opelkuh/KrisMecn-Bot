using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace KrisMecn.Extensions
{
    class EmojiExtension : BaseExtension
    {
        public DiscordEmoji[] Numbers;
        public DiscordEmoji Cancel;

        protected override void Setup(DiscordClient client)
        {
            base.Setup(client);

            Numbers = new DiscordEmoji[]
            {
                DiscordEmoji.FromName(client, ":zero:"),
                DiscordEmoji.FromName(client, ":one:"),
                DiscordEmoji.FromName(client, ":two:"),
                DiscordEmoji.FromName(client, ":three:"),
                DiscordEmoji.FromName(client, ":four:"),
                DiscordEmoji.FromName(client, ":five:"),
                DiscordEmoji.FromName(client, ":six:"),
                DiscordEmoji.FromName(client, ":seven:"),
                DiscordEmoji.FromName(client, ":eight:"),
                DiscordEmoji.FromName(client, ":nine:"),
                DiscordEmoji.FromName(client, ":keycap_ten:"),
            };
            Cancel = DiscordEmoji.FromName(client, ":x:");
        }

        public DiscordEmoji FromDigit(int number)
        {
            if(number > 10)
            {
                throw new ArgumentOutOfRangeException();
            }

            return Numbers[number];
        }

        public List<DiscordEmoji> PollNumberEmojis(int numberOfOptions, int startNum = 1, bool withCancel = true)
        {
            var ret = new List<DiscordEmoji>(numberOfOptions);

            for (int i = 0; i < numberOfOptions; i++)
            {
                ret.Add(FromDigit(i + startNum));
            }

            if(withCancel) ret.Add(Cancel);

            return ret;
        }
    }
}
