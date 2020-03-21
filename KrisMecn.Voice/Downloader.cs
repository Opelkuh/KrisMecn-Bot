using KrisMecn.Voice.Builders;
using System;
using System.Diagnostics;
using System.IO;

namespace KrisMecn.Voice
{
    public class Downloader : ChildProcessHandler
    {
        public Downloader() : base("youtube-dl", "-h") { }

        public Stream Download(string url, string format = "")
        {
            string arguments = new YoutubeDLArgBuilder()
                .Format(format)
                .Output("-") // stream to stdout
                .Build(url);

            var ytdlInfo = new ProcessStartInfo
            {
                FileName = Command,
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var ytdl = Process.Start(ytdlInfo);
            ytdl.EnableRaisingEvents = true;
            ytdl.ErrorDataReceived += OnErrorDataReceived;
            ytdl.BeginErrorReadLine();

            return ytdl.StandardOutput.BaseStream;
        }

        private static void OnErrorDataReceived(object data, DataReceivedEventArgs args)
        {
            Console.Error.WriteLine("`youtube-dl` error!");
            Console.Error.WriteLine(args.Data);
        }
    }
}
