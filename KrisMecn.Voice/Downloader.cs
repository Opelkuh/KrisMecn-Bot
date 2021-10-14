using KrisMecn.Voice.Builders;
using System.Diagnostics;
using System.IO;

namespace KrisMecn.Voice
{
    public class Downloader : ChildProcessHandler
    {
        public bool QuietOutput = true;

        public Downloader(string ytdlBinaryPath) : base(ytdlBinaryPath, "-h") { }

        public Stream Download(string url, string format = "")
        {
            // build CLI arguments
            var args = new YoutubeDLArgBuilder()
                .Format(format)
                .Output("-"); // stream to stdout

            if (QuietOutput) args.QuietOutput();

            // prepare process info
            var ytdlInfo = new ProcessStartInfo
            {
                FileName = Command,
                Arguments = args.Build(url),
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var ytdl = StartProcess(ytdlInfo);

            return ytdl.StandardOutput.BaseStream;
        }
    }
}
