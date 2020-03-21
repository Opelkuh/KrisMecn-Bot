using System.Net;
using System.Text;

namespace KrisMecn.Voice.Builders
{
    /// <summary>
    /// Generates youtube-dl CLI arguments
    /// </summary>
    internal class YoutubeDLArgBuilder
    {
        StringBuilder _sb = new StringBuilder();

        public YoutubeDLArgBuilder ListFormat()
        {
            _sb.Append("-F ");
            return this;
        }

        public YoutubeDLArgBuilder Format(string format)
        {
            // ignore empty strings
            if (format == string.Empty) return this;

            _sb.AppendFormat("-f {0} ", format);
            return this;
        }

        public YoutubeDLArgBuilder Output(string output)
        {
            _sb.AppendFormat("-o {0} ", output);
            return this;
        }

        public YoutubeDLArgBuilder ExternalDownloader(string downloaderName)
        {
            _sb.AppendFormat("--external-downloader {0} ", downloaderName);
            return this;
        }

        public YoutubeDLArgBuilder ExternalDownloaderArgs(string downloaderArgs)
        {
            _sb.AppendFormat(@"--external-downloader-args ""{0}"" ", downloaderArgs);
            return this;
        }

        public string Build(string url)
        {
            _sb.AppendFormat(@"""{0}""", url);

            return _sb.ToString();
        }
    }
}
