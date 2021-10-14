using DSharpPlus;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    class DownloaderExtension : BaseExtension
    {
        private string ytdlBinaryPath; 
        
        public DownloaderExtension(string ytdlBinaryPath) : base()
        {
            new Downloader(ytdlBinaryPath).IsAvailable();

            this.ytdlBinaryPath = ytdlBinaryPath;
        }

        public Downloader GetDownloader()
        {
            var ret = new Downloader(this.ytdlBinaryPath);
            ret.ProcessErrorEvent += Downloader_ProcessErrorEvent;

            return ret;
        }

        private void Downloader_ProcessErrorEvent(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Logger.Error("Youtube-DL Error", e.Data);
        }
    }
}
