using DSharpPlus;
using KrisMecn.Voice;
using System;
using System.Collections.Generic;
using System.Text;

namespace KrisMecn.Extensions
{
    class DownloaderExtension : BaseExtension
    {
        public DownloaderExtension() : base()
        {
            new Downloader().IsAvailable();
        }

        public Downloader GetDownloader()
        {
            var ret = new Downloader();
            ret.ProcessErrorEvent += Downloader_ProcessErrorEvent;

            return ret;
        }

        private void Downloader_ProcessErrorEvent(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Logger.Error("Youtube-DL Error", e.Data);
        }
    }
}
