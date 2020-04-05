using DSharpPlus;
using KrisMecn.Voice;

namespace KrisMecn.Extensions
{
    class ConverterExtension : BaseExtension
    {
        public Converter GetConverter()
        {
            var ret = new Converter();
            ret.ProcessErrorEvent += Converter_ProcessErrorEvent;

            return ret;
        }

        protected override void Setup(DiscordClient client)
        {
        }

        private void Converter_ProcessErrorEvent(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Logger.Error("ffmpeg Error", e.Data);
        }
    }
}
