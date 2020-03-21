using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    static class CommandContextExtensions
    {
        public async static Task PlayFromURL(this CommandContext ctx, Uri uri)
        {
            if (!uri.IsHttp()) return;

            var downloader = ctx.Client.GetDownloader();
            var converter = new Converter().ToPCM();

            var downloadStream = downloader.Download(uri.AbsoluteUri);
            var converterStream = converter.Start();

            // pass downloaded data to converter
            _ = downloadStream.CopyToAsync(converterStream.Input);

            // play audio stream
            await ctx.PlayVoiceStream(converterStream.Output).ConfigureAwait(false);
        }

        public async static Task PlayVoiceStream(this CommandContext ctx, Stream pcmStream)
        {
            var voiceConn = await ctx.GetVoiceConnection();

            // cancel the current playback if the connection is playing something
            if(voiceConn.IsPlaying)
            {
                voiceConn.StopPlayback();
            }

            var voiceStream = voiceConn.GetTransmitStream();

            _ = voiceStream.ReadFrom(pcmStream).ConfigureAwait(false);
        }

        public async static Task<VoiceNextConnection> GetVoiceConnection(this CommandContext ctx)
        {
            // ignore non-public channels
            if (ctx.Guild == null) return null;

            // fetch voice library
            var vclient = ctx.Client.GetVoiceNext();
            if(vclient == null)
            {
                throw new Exception("Failed to load voice module");
            }

            var existingConn = vclient.GetConnection(ctx.Guild);

            // return old connection if it exists
            if (existingConn != null) return existingConn;

            // create a new connection
            var targetChannel = ctx.Member?.VoiceState?.Channel;

            // ignore if the user is not connected to a voice channel
            if (targetChannel == null) return null;

            var newConn = await vclient.ConnectAsync(targetChannel);

            Logger.Info($"Connected to voice channel - {targetChannel.Guild.Name} : {targetChannel.Name}");

            newConn.VoiceSocketErrored += VoiceConnection_VoiceSocketErrored;

            return newConn;
        }

        public async static Task DisconnectVoiceConnection(this CommandContext ctx)
        {
            // ignore non-public channels
            if (ctx.Guild == null) return;

            // fetch voice library
            var vclient = ctx.Client.GetVoiceNext();
            if (vclient == null)
            {
                throw new Exception("Failed to load voice module");
            }

            var existingConn = vclient.GetConnection(ctx.Guild);

            // ignore if the client isn't connected
            if (existingConn == null) return;

            await existingConn.SendSpeakingAsync(false);
            existingConn.Disconnect();

            Logger.Info($"Disconnected from voice channel - {existingConn.Channel.Guild.Name} : {existingConn.Channel.Name}");
        }

        private static Task VoiceConnection_VoiceSocketErrored(DSharpPlus.EventArgs.SocketErrorEventArgs e)
        {
            Logger.Error(e.ToString());
            return Task.CompletedTask;
        }
    }
}
