using System.Collections.Generic;
using System.Text;

namespace KrisMecn.Voice.Builders
{
    /// <summary>
    /// Generates youtube-dl CLI arguments
    /// </summary>
    internal class FfmpegArgBuilder
    {
        StringBuilder _sb = new StringBuilder();
        List<string> _audioFilters = new List<string>();

        public FfmpegArgBuilder Input(string input)
        {
            _sb.AppendFormat(@"-i ""{0}"" ", input);
            return this;
        }

        public FfmpegArgBuilder LogLevel(FfmpegLogLevel level)
        {
            _sb.AppendFormat("-loglevel {0} ", level.GetHashCode());
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

        public FfmpegArgBuilder AddAudioFilter(string name, double value)
            => AddAudioFilter(name, value.ToString());
        public FfmpegArgBuilder AddAudioFilter(string name, string value)
        {
            _audioFilters.Add($"{name}={value}");
            return this;
        }

        public FfmpegArgBuilder Format(string format)
        {
            _sb.AppendFormat("-f {0} ", format);
            return this;
        }

        public string Build(string output)
        {
            // add all audio filters
            if (_audioFilters.Count > 0)
            {
                _sb.Append("-filter:a ");

                var lastIndex = _audioFilters.Count - 1;
                for (int i = 0; i < lastIndex; i++)
                {
                    _sb.AppendFormat("{0},", _audioFilters[i]);
                }

                // append last without comma
                _sb.AppendFormat("{0} ", _audioFilters[lastIndex]);
            }

            _sb.Append(output);

            return _sb.ToString();
        }
    }

    public enum FfmpegLogLevel
    {
        Quiet = -8,
        Panic = 0,
        Fatal = 8,
        Error = 16,
        Warning = 24,
        Info = 32,
        Verbose = 40,
        Debug = 48,
        Trace = 56,
    }
}
