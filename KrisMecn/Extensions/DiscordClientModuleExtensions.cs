using DSharpPlus;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    static class DiscordClientModuleExtensions
    {
        public static Downloader GetDownloader(this DiscordClient client) => client.GetExtension<DownloaderExtension>().Downloader;
    }
}
