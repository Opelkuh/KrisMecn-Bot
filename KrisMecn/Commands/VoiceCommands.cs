using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System.Linq;
using KrisMecn.Extensions;
using System;
using System.Collections.Generic;
using DSharpPlus.Entities;
using System.Text;
using KrisMecn.Voice;

namespace KrisMecn.Commands
{
    public class VoiceCommands : BaseKrisCommandModule
    {
        private readonly static HashSet<string> ALLOWED_YOUTUBE_HOSTS = new HashSet<string>(
            new string[]
            {
                "youtube.com",
                "m.youtube.com",
                "www.youtube.com",
                "music.youtube.com",
                "youtu.be"
            }
        );

        [
            Command("play"),
            Aliases("p", "pl"),
            Description("Plays audio from the provided link in your current voice channel")
        ]
        public async Task Play(CommandContext ctx, Uri url)
        {
            await ctx.PlayFromURL(url).ConfigureAwait(false);
        }

        [
            Command("nightcore"),
            Aliases("nc", "playnightcore"),
            Description(@"Plays """"""nightcore"""""" from the provided link in your current voice channel")
        ]
        public async Task PlayNightcore(CommandContext ctx, Uri url)
        {
            var converter = new Converter().IncreaseTempo(1.25);

            await ctx.PlayFromURL(url, converter).ConfigureAwait(false);
        }

        [
            Command("vaporwave"),
            Aliases("vw", "playvaporwave"),
            Description(@"Plays """"""vaporwave"""""" from the provided link in your current voice channel")
        ]
        public async Task PlayVaporwave(CommandContext ctx, Uri url)
        {
            var converter = new Converter().IncreaseTempo(0.65);

            await ctx.PlayFromURL(url, converter).ConfigureAwait(false);
        }

        [
            Command("yt"),
            Aliases("youtube"),
            Description("Plays a YouTube video in your current voice channel")
        ]
        public async Task PlayYoutube(CommandContext ctx, [RemainingText] string urlOrQuery)
        {
            Uri youtubeUri;

            if (
                Uri.TryCreate(urlOrQuery, UriKind.Absolute, out youtubeUri) &&
                youtubeUri.IsHttp() &&
                ALLOWED_YOUTUBE_HOSTS.Contains(youtubeUri.Host)
            )
            {
                await ctx.PlayFromURL(youtubeUri).ConfigureAwait(false);
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();
            var emoji = ctx.Client.GetEmojis();
            var youtubeApi = ctx.Client.GetYoutubeAPI();

            var searchRes = await youtubeApi.Search(urlOrQuery);
            var searchResNum = searchRes.Count;

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Search results for: `{urlOrQuery}`")
                .WithAuthorFooter(ctx.Member, "Requested by: ")
                .WithColor(new DiscordColor("#FF0000"))
                .WithUrl($"https://www.youtube.com/results?search_query={Uri.EscapeUriString(urlOrQuery)}");

            if (searchResNum > 0)
            {
                var description = new StringBuilder();
                for (int i = 0; i < searchResNum; i++)
                {
                    description.AppendFormat("**{0}.** `{1}`", i + 1, searchRes[i].Snippet.Title);
                    description.AppendLine();
                }
                embed.WithDescription(description.ToString());
            }
            else
            {
                embed.WithDescription("***Nothing***");
            }

            var resultMessage = await ctx.RespondAsync(embed: embed);
            var emojis = emoji.PollNumberEmojis(searchResNum);

            int selectedVid = await resultMessage.PollUserAsync(ctx.Member, emojis.ToArray(), 30);

            if (0 <= selectedVid && selectedVid < searchResNum)
            {
                var videoId = searchRes[selectedVid].Id.VideoId;
                var videoUri = new Uri($"https://youtu.be/{videoId}");

                await ctx.PlayFromURL(videoUri);
            }

            await resultMessage.DeleteAsync();
        }

        [
            Command("dc"),
            Aliases("disconnect"),
            Description("Disconnects the bot from voice channels")
        ]
        public async Task Disconnect(CommandContext ctx)
        {
            await ctx.DisconnectVoiceConnection();
        }
    }
}