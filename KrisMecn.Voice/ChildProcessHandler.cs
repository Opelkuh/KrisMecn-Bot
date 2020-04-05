using System;
using System.Diagnostics;

namespace KrisMecn.Voice
{
    public abstract class ChildProcessHandler
    {
        protected string Command;

        private string _testArguments;

        public event EventHandler<DataReceivedEventArgs> ProcessErrorEvent;

        internal ChildProcessHandler(string command, string testArguments = "")
        {
            Command = command;
            _testArguments = testArguments;
        }

        public void IsAvailable()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = Command,
                    Arguments = _testArguments,
                    CreateNoWindow = true,
                };

                var process = Process.Start(processInfo);

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"`{Command}` returned non-zero code!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($"Failed to load `{Command}`. Make sure it's installed and in PATH");
                return;
            }
        }

        protected virtual void OnErrorDataReceived(object data, DataReceivedEventArgs args)
            => ProcessErrorEvent?.Invoke(data, args);
    }
}
