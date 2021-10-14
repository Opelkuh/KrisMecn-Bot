using DSharpPlus;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    class ConverterExtension : BaseExtension
    {
        private string ffmpegBinaryPath; 
        
        public ConverterExtension(string ffmpegBinaryPath)
        {
            new Converter(ffmpegBinaryPath).IsAvailable();

            this.ffmpegBinaryPath = ffmpegBinaryPath;
        }

        public Converter GetConverter()
        {
            var ret = new Converter(this.ffmpegBinaryPath);
            ret.ProcessErrorEvent += Converter_ProcessErrorEvent;

            return ret;
        }

        public Converter GetConverter(string filePath)
        {
            var ret = new Converter(this.ffmpegBinaryPath, filePath);
            ret.ProcessErrorEvent += Converter_ProcessErrorEvent;

            return ret;
        }

        private void Converter_ProcessErrorEvent(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Logger.Error("ffmpeg Error", e.Data);
        }
    }
}
