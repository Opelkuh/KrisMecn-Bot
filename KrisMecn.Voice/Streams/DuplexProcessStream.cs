using System;
using System.Diagnostics;
using System.IO;

namespace KrisMecn.Voice.Streams
{
    public class DuplexProcessStream
    {
        public readonly Stream Input;
        public readonly Stream Output;

        internal DuplexProcessStream(Stream input, Stream output)
        {
            Input = input;
            Output = output;
        }
    }
}
