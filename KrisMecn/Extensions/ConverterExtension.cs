using DSharpPlus;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    class ConverterExtension : BaseExtension
    {
        public ConverterExtension()
        {
            new Converter().IsAvailable();
        }

        public Converter GetConverter()
        {
            var ret = new Converter();
            ret.ProcessErrorEvent += Converter_ProcessErrorEvent;

            return ret;
        }

        public Converter GetConverter(string filePath)
        {
            var ret = new Converter(filePath);
            ret.ProcessErrorEvent += Converter_ProcessErrorEvent;

            return ret;
        }

        private void Converter_ProcessErrorEvent(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Logger.Error("ffmpeg Error", e.Data);
        }
    }
}
