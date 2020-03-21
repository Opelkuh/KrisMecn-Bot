using KrisMecn.Voice.Builders;
using KrisMecn.Voice.Streams;
using System;
using System.Diagnostics;

namespace KrisMecn.Voice
{
    public class Converter : ChildProcessHandler
    {
        private ProcessStartInfo _info;
        private FfmpegArgBuilder _argBuilder;

        public Converter() : base("ffmpeg", "-h")
        {
            _argBuilder = new FfmpegArgBuilder()
                .Input("-"); // we will pass input with stdin

            _info = new ProcessStartInfo
            {
                FileName = Command,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
        }

        public Converter ToPCM() => ToPCM(2, 48000);
        public Converter ToPCM(byte channels, int hz)
        {
            _argBuilder
                .AudioChannels(channels)
                .AudioRate(hz)
                .Format("s16le");

            return this;
        }

        public DuplexProcessStream Start()
        {
            _info.Arguments = _argBuilder.Build("pipe:1"); // pipe to stdout

            var ffmpeg = Process.Start(_info);

            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.ErrorDataReceived += OnErrorDataReceived;
            ffmpeg.Exited += (a, b) =>
            {
                Console.WriteLine("ffmpeg exited {0}", ffmpeg.ExitCode);
            };
            ffmpeg.BeginErrorReadLine();

            return new DuplexProcessStream(
                ffmpeg.StandardInput.BaseStream,
                ffmpeg.StandardOutput.BaseStream
            );
        }

        private static void OnErrorDataReceived(object data, DataReceivedEventArgs args)
        {
            Console.Error.WriteLine("`ffmpeg` error!");
            Console.Error.WriteLine(args.Data);
        }
    }
}
