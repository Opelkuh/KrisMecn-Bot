using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using DSharpPlus.Entities;
using System.Text;
using KrisMecn.Extensions;
using KrisMecn.Attributes;
using KrisMecn.Helpers.Extensions;

namespace KrisMecn.Commands
{
    [RequireVoiceChannel]
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
            Description("Plays audio from the provided link in your current voice channel. Supports all sites that are supported by `youtube-dl`")
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
            var converter = ctx.Client.GetConverter().IncreaseTempo(1.25);

            await ctx.PlayFromURL(url, converter).ConfigureAwait(false);
        }

        [
            Command("vaporwave"),
            Aliases("vw", "playvaporwave"),
            Description(@"Plays """"""vaporwave"""""" from the provided link in your current voice channel")
        ]
        public async Task PlayVaporwave(CommandContext ctx, Uri url)
        {
            var converter = ctx.Client.GetConverter().IncreaseTempo(0.65);

            await ctx.PlayFromURL(url, converter).ConfigureAwait(false);
        }

        [
            Command("yt"),
            Aliases("youtube"),
            Description("Searches YouTube with the provided query and lets you select the video that you want to play.")
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

            // delete poll message
            await resultMessage.DeleteAsync();

            // play selected video (if anything was selected)
            if (selectedVid < 0 || selectedVid >= searchResNum) return;

            var videoId = searchRes[selectedVid].Id.VideoId;
            var videoUri = new Uri($"https://youtu.be/{videoId}");

            await ctx.PlayFromURL(videoUri);
        }

        [
            Command("volume"),
            Aliases("v"),
            Description("Changes the volume of the song that's currently playing")
        ]
        public async Task Volume(CommandContext ctx, string percentage)
        {
            double outVolume;
            switch(percentage)
            {
                case "doprava":
                    outVolume = 6;
                    break;
                default:
                    double userInput;
                    if (!double.TryParse(percentage, out userInput)) return;

                    // clamp to max value of 600%
                    outVolume = Math.Clamp(userInput / 100, 0, 6);

                    break;
            }
            
            // change volume
            var voiceCon = await ctx.GetVoiceConnection(true);

            if (voiceCon == null) return;

            voiceCon.GetTransmitStream().VolumeModifier = outVolume;
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