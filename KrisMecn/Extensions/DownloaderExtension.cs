using DSharpPlus;
using KrisMecn.Voice;
using System;
using System.Collections.Generic;
using System.Text;

namespace KrisMecn.Extensions
{
    class DownloaderExtension : BaseExtension
    {
        public Downloader Downloader { get; private set; }

        public DownloaderExtension() : base()
        {
            Downloader = new Downloader();
            
            Downloader.IsAvailable();

            Downloader.ProcessErrorEvent += Downloader_ProcessErrorEvent;
        }

        protected override void Setup(DiscordClient client)
        {
        }

        private void Downloader_ProcessErrorEvent(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Logger.Error("Youtube-DL Error", e.Data);
        }
    }
}
