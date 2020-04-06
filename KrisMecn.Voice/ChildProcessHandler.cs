using System;
using System.Diagnostics;

namespace KrisMecn.Voice
{
    public abstract class ChildProcessHandler : IDisposable
    {
        protected string Command;

        private string _testArguments;

        public event EventHandler<DataReceivedEventArgs> ProcessErrorEvent;

        protected Process RunningProcess { get; private set; }

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

        protected virtual Process StartProcess(ProcessStartInfo info)
        {
            RunningProcess = Process.Start(info);
            RunningProcess.EnableRaisingEvents = true;
            RunningProcess.ErrorDataReceived += OnErrorDataReceived;
            RunningProcess.Exited += (a, b) =>
            {
                Console.WriteLine($"Process {Command} exited {0}", RunningProcess.ExitCode);
            };
            RunningProcess.BeginErrorReadLine();

            return RunningProcess;
        }

        protected virtual void OnErrorDataReceived(object data, DataReceivedEventArgs args)
            => ProcessErrorEvent?.Invoke(data, args);

        public void Dispose()
        {
            if (RunningProcess != null)
                RunningProcess.Dispose();
        }
    }
}
