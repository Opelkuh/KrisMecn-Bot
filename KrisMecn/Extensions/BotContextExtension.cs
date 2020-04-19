using DSharpPlus;

namespace KrisMecn.Extensions
{
    class BotContextExtension : BaseExtension
    {
        public Bot BotInstance { get; }

        public BotContextExtension(Bot bot) => BotInstance = bot;

        protected override void Setup(DiscordClient client)
        {
        }
    }
}
