using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using KrisMecn.Entities;
using KrisMecn.Extensions;
using System;
using System.IO;
using System.Reflection;
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

        public Bot()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                new Config().SaveToFile(CONFIG_PATH);

                Console.WriteLine($"{CONFIG_PATH} not found, new one has been generated. Please fill in your info");
                return;
            }

            var config = Config.LoadFromFile(CONFIG_PATH);

            // setup discord client
            _client = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,

                UseInternalLogHandler = true,
                AutoReconnect = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
            });

            // setup commands
            _commands = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { config.Prefix },
                CaseSensitive = false,        
                EnableDms = true,
                EnableDefaultHelp = false,
            });

            // setup interactivity
            _interactivity =_client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromSeconds(30)
            });

            // setup voice
            _voice = _client.UseVoiceNext(new VoiceNextConfiguration()
            {
                EnableIncoming = false,
            });

            // setup custom modules
            _client.AddExtension(new EmojiExtension());
            _client.AddExtension(new DownloaderExtension());
            _client.AddExtension(new BooruExtension());
            _client.AddExtension(new YoutubeAPIExtension(config.GoogleApiKey));

            // hook events
            _client.Ready += Client_Ready;
            _client.ClientErrored += Client_ClientError;

            _commands.CommandExecuted += Commands_CommandExecuted;
            _commands.CommandErrored += Commands_CommandErrored;

            // add commands
            _commands.RegisterCommands(Assembly.GetExecutingAssembly());
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            Logger.Info("Client is ready to process events.");

            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            Logger.Error($"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}");

            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Logger.Info($"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");

            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            Logger.Error(
                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}"
            );

            // check if the error is a result of lack of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                await e.Context.Member.SendMessageAsync("You don't have the required permission");  
            }
        }

        public async Task StartAsync()
        {
            await _client.ConnectAsync();

            /*
            var download = Downloader.Download("https://macross82-99.bandcamp.com/album/shibuya-meltdown", "");
            var convert = new Converter().ToPCM().Start();

            convert.ReadFrom(download);

            using(var ms = new MemoryStream())
            {
                int totalRead = 0;
                var buff = new byte[3840];
                while (true)
                {
                    var read = await convert.Output.ReadAsync(buff);

                    totalRead += read;
                    Console.WriteLine("Read {0}. Total: {1}", read, totalRead);

                    if (read == 0) break;
                }
            }
            */

            await Task.Delay(-1);
        }
    }
}
