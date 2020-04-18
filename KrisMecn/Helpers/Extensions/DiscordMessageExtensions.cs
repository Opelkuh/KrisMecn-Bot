using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KrisMecn.Helpers.Extensions
{
    static class DiscordMessageExtensions
    {
        public async static Task<int> PollUserAsync(this DiscordMessage msg, DiscordUser user, DiscordEmoji[] emoji, int timeout)
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
                    if (emoji[i].Id == re.Id && emoji[i].Name == re.Name) return i;
                }

                // try again, adjust timeout for the duration that the user took to react
                var elapsedTime = DateTime.Now - start;
                timeoutSpan = TimeSpan.FromSeconds(timeout - elapsedTime.TotalSeconds);
            }

            tokenSource.Cancel();
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
    }
}
