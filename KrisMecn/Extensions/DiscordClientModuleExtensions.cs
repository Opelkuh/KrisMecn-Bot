using DSharpPlus;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    static class DiscordClientModuleExtensions
    {
        public static Downloader GetDownloader(this DiscordClient client) => client.GetExtension<DownloaderExtension>().Downloader;
        public static YoutubeAPIExtension GetYoutubeAPI(this DiscordClient client) => client.GetExtension<YoutubeAPIExtension>();
        public static EmojiExtension GetEmojis(this DiscordClient client) => client.GetExtension<EmojiExtension>();
        public static BooruExtension GetBooru(this DiscordClient client) => client.GetExtension<BooruExtension>();
    }
}
