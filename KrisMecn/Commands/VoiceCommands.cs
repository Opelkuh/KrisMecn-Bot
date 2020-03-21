using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using KrisMecn.Extensions;
using KrisMecn.Voice;
using System;
using System.Collections.Generic;

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
            Command("yt"),
            Aliases("youtube"),
            Description("Plays a YouTube video in your current voice channel")
        ]
        public async Task PlayYoutube(CommandContext ctx, Uri url)
        {
            // check if the provided url is a youtube link
            if(!url.IsHttp() || !ALLOWED_YOUTUBE_HOSTS.Contains(url.Host))
            {
                Logger.Info($"{ctx.User.Username}#{ctx.User.Discriminator} tried to execute `yt` with invalid URL: {url}");
                return;
            }

            await ctx.PlayFromURL(url).ConfigureAwait(false);
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