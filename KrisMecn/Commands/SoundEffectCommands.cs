using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Entities;
using System;
using System.Threading.Tasks;
using KrisMecn.Helpers.Extensions;
using DSharpPlus.Entities;

namespace KrisMecn.Commands
{
    /// <summary>
    /// Sound effect commands module stub. Commands in this module are registered in 
    /// </summary>
    public class SoundEffectCommands : BaseKrisCommandModule, ICommandModule
    {
        public delegate Task DSharpPlusCommand(CommandContext ctx);

        public Type ModuleType => GetType();
        public BaseCommandModule GetInstance(IServiceProvider services)
            => this;

        public DSharpPlusCommand GetPlaySoundEffectCommand(string filePath)
        {
            return async (ctx) =>
            {
                try
                {
                    await ctx.Message.DeleteAsync();
                } catch(Exception) { }

                var playbackInfo = ctx.GeneratePlaybackInfoEmbed()
                    .WithDescription("Built-in sound file")
                    .WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl)
                    .WithColor(new DiscordColor("#EBA991"));

                await ctx.PlayFromFile(filePath, playbackInfo).ConfigureAwait(false);
            };
        }
    }
}
