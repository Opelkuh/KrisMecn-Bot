﻿using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KrisMecn.Helpers.Extensions
{
    static class DiscordMessageExtensions
    {
        public async static Task<int> PollUserAsync(this DiscordMessage msg, DiscordUser user, DiscordEmoji[] emoji, int timeout, bool removeReactions = false)
        {
            var tokenSource = new CancellationTokenSource();
            _ = msg.CreateReactionsAsync(emoji, tokenSource.Token);

            var start = DateTime.Now;
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            while (timeoutSpan.TotalSeconds > 0)
            {
                // wait for user's reaction
                var reaction = await msg.WaitForReactionAsync(user, timeoutSpan);

                // cancel if the user didn't react
                if (reaction.TimedOut) break;

                // find if the user reacted with an emoji that we expect
                for (int i = 0; i < emoji.Length; i++)
                {
                    var re = reaction.Result.Emoji;
                    if (emoji[i].Id == re.Id && emoji[i].Name == re.Name)
                    {
                        tokenSource.Cancel();

                        if (removeReactions) await msg.DeleteOwnReactionsAsync(emoji);

                        return i;
                    }
                }

                // try again, adjust timeout for the duration that the user took to react
                var elapsedTime = DateTime.Now - start;
                timeoutSpan = TimeSpan.FromSeconds(timeout - elapsedTime.TotalSeconds);
            }

            tokenSource.Cancel();

            // remove reactions if requested
            if (removeReactions) await msg.DeleteOwnReactionsAsync(emoji);

            return -1;
        }

        public async static Task CreateReactionsAsync(this DiscordMessage msg, IEnumerable<DiscordEmoji> emojis, CancellationToken cancellationToken)
        {
            foreach (var emoji in emojis)
            {
                if (cancellationToken.IsCancellationRequested) return;

                await msg.CreateReactionAsync(emoji);
            }
        }

        public async static Task DeleteOwnReactionsAsync(this DiscordMessage msg, IEnumerable<DiscordEmoji> emojis)
        {
            foreach (var emoji in emojis)
            {
                await msg.DeleteOwnReactionAsync(emoji);
            }
        }
    }
}
