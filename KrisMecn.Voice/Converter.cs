using KrisMecn.Voice.Builders;
using KrisMecn.Voice.Streams;
using System.Diagnostics;

namespace KrisMecn.Voice
{
    public class Converter : ChildProcessHandler
    {
        private const FfmpegLogLevel DEFAULT_LOG_LEVEL = FfmpegLogLevel.Warning;

        private ProcessStartInfo _info;
        private FfmpegArgBuilder _argBuilder;

        public Converter() : this(DEFAULT_LOG_LEVEL) { }
        public Converter(string inputPath) : this(DEFAULT_LOG_LEVEL, inputPath) { }
        public Converter(FfmpegLogLevel logLevel) : this(logLevel, "-") { } // default to stdin input
        public Converter(FfmpegLogLevel logLevel, string inputPath) : base("ffmpeg", "-hide_banner -h")
        {
            _argBuilder = new FfmpegArgBuilder()
                .Input(inputPath)
                .LogLevel(logLevel);

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

        public Converter IncreaseTempo(double rate, int inputSamplingRate = 44100)
        {
            _argBuilder.AddAudioFilter("asetrate", rate * inputSamplingRate);
            return this;
        }

        public DuplexProcessStream Start()
        {
            _info.Arguments = _argBuilder.Build("pipe::"); // pipe to stdout

            var ffmpeg = StartProcess(_info);

            return new DuplexProcessStream(
                ffmpeg.StandardInput.BaseStream,
                ffmpeg.StandardOutput.BaseStream
            );
        }
    }
}
