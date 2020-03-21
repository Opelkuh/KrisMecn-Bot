using System.Text;

namespace KrisMecn.Voice.Builders
{
    /// <summary>
    /// Generates youtube-dl CLI arguments
    /// </summary>
    internal class FfmpegArgBuilder
    {
        StringBuilder _sb = new StringBuilder();

        public FfmpegArgBuilder Input(string input)
        {
            _sb.AppendFormat("-i {0} ", input);
            return this;
        }

        public FfmpegArgBuilder AudioChannels(byte numChannels)
        {
            _sb.AppendFormat("-ac {0} ", numChannels);
            return this;
        }

        public FfmpegArgBuilder AudioRate(int audioRate)
        {
            _sb.AppendFormat("-ar {0} ", audioRate);
            return this;
        }

        public FfmpegArgBuilder Format(string format)
        {
            _sb.AppendFormat("-f {0} ", format);
            return this;
        }

        public string Build(string output)
        {
            _sb.Append(output);

            return _sb.ToString();
        }
    }
}
