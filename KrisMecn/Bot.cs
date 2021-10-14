using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using KrisMecn.Attributes;
using KrisMecn.Entities;
using KrisMecn.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KrisMecn
{
    class Bot
    {
        private const string CONFIG_PATH = "config.json";
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private InteractivityExtension _interactivity;
        private VoiceNextExtension _voice;

        public StartTimes StartTimes { get; } = new StartTimes();
        public Config Config { get; }

        public Bot()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                new Config().SaveToFile(CONFIG_PATH);

                Console.WriteLine($"{CONFIG_PATH} not found, new one has been generated. Please fill in your info");
                return;
            }

            Config = Config.LoadFromFile(CONFIG_PATH);

            // setup discord client
            _client = new DiscordClient(new DiscordConfiguration()
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
            });

            // setup commands
            _commands = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { Config.Prefix },
                CaseSensitive = false,
                EnableDms = true,
                EnableDefaultHelp = false,
            });

            // setup interactivity
            _interactivity = _client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromSeconds(30)
            });

            // setup voice
            _voice = _client.UseVoiceNext(new VoiceNextConfiguration()
            {
                EnableIncoming = false,
            });

            // setup custom extensions
            _client.AddExtension(new BotContextExtension(this));
            _client.AddExtension(new EmojiExtension());
            _client.AddExtension(new DownloaderExtension(Config.YoutubeDlBinaryPath));
            _client.AddExtension(new ConverterExtension(Config.FFmpegBinaryPath));
            _client.AddExtension(new BooruExtension());
            _client.AddExtension(new SoundEffectWatcherExtension("./soundEffects"));
            _client.AddExtension(new YoutubeAPIExtension(Config.GoogleApiKey));

            // hook events
            _client.Ready += Client_Ready;
            _client.SocketOpened += Client_SocketOpened;
            _client.SocketClosed += Client_SocketClosed;
            _client.SocketErrored += Client_SocketErrored;
            _client.ClientErrored += Client_ClientError;

            _commands.CommandExecuted += Commands_CommandExecuted;
            _commands.CommandErrored += Commands_CommandErrored;

            // add commands
            _commands.RegisterCommands(Assembly.GetExecutingAssembly());
        }

        private Task Client_Ready(DiscordClient c, ReadyEventArgs e)
        {
            Logger.Info("Client is ready to process events.");

            return Task.CompletedTask;
        }

        private Task Client_SocketOpened(DiscordClient c, SocketEventArgs e)
        {
            StartTimes.SocketStart = DateTime.Now;

            Logger.Info("Gateway connection established");

            return Task.CompletedTask;
        }

        private Task Client_SocketClosed(DiscordClient c, SocketCloseEventArgs e)
        {
            Logger.Info($"Gateway connection closed. Code: {e.CloseCode}");

            return Task.CompletedTask;
        }

        private Task Client_SocketErrored(DiscordClient c, SocketErrorEventArgs e)
        {
            Logger.Error("Gateway connection error", e.Exception);

            return Task.CompletedTask;
        }


        private Task Client_ClientError(DiscordClient c, ClientErrorEventArgs e)
        {
            Logger.Error($"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}");

            return Task.CompletedTask;
        }

        private async Task Commands_CommandExecuted(CommandsNextExtension s, CommandExecutionEventArgs e)
        {
            try
            {
                // delete the users command
                await e.Context.Message.DeleteAsync();
            }
            catch { }

            // log that the command was finished
            Logger.Info($"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");
        }

        private async Task Commands_CommandErrored(CommandsNextExtension s, CommandErrorEventArgs e)
        {
            // log error
            Logger.Error(
                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}"
            );

            // check if the error is a result of lack of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                if (e.Context.Member == null || ex.FailedChecks.Count <= 0) return;

                var sb = new StringBuilder();
                foreach (var check in ex.FailedChecks)
                {
                    switch (check)
                    {
                        case RequireNsfwAttribute _:
                            sb.Append($"Channel #{ex.Context.Channel.Name} isn't set as NSFW.\n");
                            break;
                        case RequireVoiceChannelAttribute _:
                            sb.Append("You have to be connected to a voice channel.\n");
                            break;
                    }
                }

                if (sb.Length > 0)
                {
                    await e.Context.Member.SendMessageAsync(sb.ToString());
                    try
                    {
                        // delete the users command
                        await e.Context.Message.DeleteAsync();
                    }
                    catch { }
                }
            }
        }

        public async Task StartAsync()
        {
            // prepare activity
            var activity = new DiscordActivity();
            activity.ActivityType = Config.Activity.Type;
            activity.Name = Config.Activity.Text;

            // start connection
            await _client.ConnectAsync(activity, Config.Activity.Status);

            await Task.Delay(-1);
        }
    }
}
