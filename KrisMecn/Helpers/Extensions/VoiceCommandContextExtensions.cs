using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.EventArgs;
using KrisMecn.Voice;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KrisMecn.Helpers.Extensions
{
    static class VoiceCommandContextExtensions
    {
        public async static Task PlayFromURL(this CommandContext ctx, Uri uri, DiscordEmbed playbackInfo = null, Converter converter = null)
        {
            if (!uri.IsHttp())
            {
                Logger.Info($"{ctx.User.Username}#{ctx.User.Discriminator} tried to play a music stream from invalid URL: {uri}");
                return;
            }

            // create converter if none is provided
            if (converter == null) converter = ctx.Client.GetConverter();
            converter.ToPCM(); // set output to PCM

            // prepare downloader
            var downloader = ctx.Client.GetDownloader();

            // start all processes
            try
            {
                // start download
                var downloadStream = downloader.Download(uri.AbsoluteUri);
                var converterStream = converter.Start();

                // pass downloaded data to converter and play the converted output
                var downloadStreamTask = downloadStream.CopyToAsync(converterStream.Input, 4096000 /* ~4mb */);
                var playTask = ctx.PlayVoiceStream(converterStream.Output, playbackInfo);

                // wait for one of the tasks to finish or fail
                var anyTask = await Task.WhenAny(downloadStreamTask, playTask);

                if (anyTask.Exception != null) throw anyTask.Exception;
                if (anyTask.IsCanceled) throw new TaskCanceledException();

                // close converter input
                await converterStream.Input.FlushAsync();
                converterStream.Input.Close();

                // wait for all tasks to finish
                await Task.WhenAll(downloadStreamTask, playTask);
            }
            catch (Exception e)
            {
                Logger.Error("PlayFromURL error", e);
            }
            finally
            {
                downloader.Dispose();
                converter.Dispose();
            }
        }

        public async static Task PlayFromFile(this CommandContext ctx, string path, DiscordEmbed playbackInfo = null)
        {
            using (var converter = ctx.Client.GetConverter(path).ToPCM())
            {
                var converterStream = converter.Start();

                await ctx.PlayVoiceStream(converterStream.Output, playbackInfo);
            }
        }

        public async static Task PlayVoiceStream(this CommandContext ctx, Stream pcmStream, DiscordEmbed playbackInfo = null)
        {
            var voiceConn = await ctx.GetVoiceConnection();

            // cancel the current playback if the connection is playing something
            if (voiceConn.IsPlaying)
            {
                voiceConn.StopPlayback();
            }

            var voiceStream = voiceConn.GetTransmitStream();

            // reset volume to 1
            voiceStream.VolumeModifier = 1;

            await voiceStream.ReadFrom(pcmStream, playbackInfo);
        }

        public async static Task<DiscordEmbed> GetCurrentPlaybackInfo(this CommandContext ctx)
        {
            var voiceConn = await ctx.GetVoiceConnection(true);

            return voiceConn?.GetTransmitStream()?.CurrentPlaybackInfo;
        }

        public async static Task<VoiceNextConnection> GetVoiceConnection(this CommandContext ctx, bool onlyExisting = false)
        {
            // ignore non-public channels
            if (ctx.Guild == null) return null;

            // fetch voice library
            var vclient = ctx.Client.GetVoiceNext();
            if (vclient == null)
            {
                throw new Exception("Failed to load voice module");
            }

            var existingConn = vclient.GetConnection(ctx.Guild);

            // return old connection if it exists
            if (existingConn != null) return existingConn;

            // don't create a new one if `onlyExisting` is set
            if (onlyExisting) return null;

            // create a new connection
            var targetChannel = ctx.Member?.VoiceState?.Channel;

            // ignore if the user is not connected to a voice channel
            if (targetChannel == null) return null;

            var newConn = await vclient.ConnectAsync(targetChannel);

            Logger.Info($"Connected to voice channel - {targetChannel.Guild.Name} : {targetChannel.Name}");

            newConn.VoiceSocketErrored += VoiceConnection_VoiceSocketErrored;
            newConn.UserLeft += VoiceConnection_UserLeft;

            return newConn;
        }

        public static Task DisconnectVoiceConnection(this CommandContext ctx)
        {
            // ignore non-public channels
            if (ctx.Guild == null) return Task.CompletedTask;

            // fetch voice library
            var vclient = ctx.Client.GetVoiceNext();
            if (vclient == null)
            {
                throw new Exception("Failed to load voice module");
            }

            var existingConn = vclient.GetConnection(ctx.Guild);

            // ignore if the client isn't connected
            if (existingConn == null) return Task.CompletedTask;

            existingConn.Disconnect();

            Logger.Info($"Disconnected from voice channel - {existingConn.Channel.Guild.Name} : {existingConn.Channel.Name}");

            return Task.CompletedTask;
        }

        public static DiscordEmbedBuilder GeneratePlaybackInfoEmbed(this CommandContext ctx, Uri url = null)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{ctx.Prefix}{ctx.Command.QualifiedName}")
                .WithAuthorFooter(ctx.Member, "Requested by: ")
                .WithTimestamp(DateTime.Now);

            if (url != null)
            {
                embed.WithDescription(url.AbsoluteUri);
            }

            return embed;
        }

        private static Task VoiceConnection_VoiceSocketErrored(VoiceNextConnection sender, SocketErrorEventArgs e)
        {
            Logger.Error(e);
            return Task.CompletedTask;
        }

        private static Task VoiceConnection_UserLeft(VoiceNextConnection conn, VoiceUserLeaveEventArgs e)
        {
            // ignore event if the channel isn't empty
            if (!IsEmptyVoiceChannel(conn.Channel, e.User)) return Task.CompletedTask;

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));

                if (!IsEmptyVoiceChannel(conn.Channel)) return;

                conn.Disconnect();
            });

            // don't wait for the delayed task
            return Task.CompletedTask;
        }

        private static bool IsEmptyVoiceChannel(DiscordChannel channel, DiscordUser ignoredUser = null)
        {
            if (channel.Type != DSharpPlus.ChannelType.Voice) throw new Exception("IsEmptyChannel called with non-voice channel");

            foreach (var user in channel.Users)
            {
                // return false on first non-bot user
                if (!user.IsBot && user.Id != ignoredUser?.Id) return false;
            }

            return true;
        }
    }
}
