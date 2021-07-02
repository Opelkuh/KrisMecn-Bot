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
                    RedirectStandardOutput = true,
                };

                var process = Process.Start(processInfo);

                _ = process.StandardOutput.ReadToEnd();
                process.WaitForExit(3000);

                if (process.ExitCode != 0)
                {
                    throw new Exception($"`{Command}` returned non-zero code ({process.ExitCode})!");
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
            RunningProcess.BeginErrorReadLine();

            return RunningProcess;
        }

        protected virtual void OnErrorDataReceived(object data, DataReceivedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Data)) return;

            ProcessErrorEvent?.Invoke(data, args);
        }

        public void Dispose()
        {
            if (RunningProcess != null)
            {
                try
                {
                    RunningProcess.Kill();
                }
                catch (Exception) { }

                RunningProcess.Dispose();
            }
        }
    }
}
