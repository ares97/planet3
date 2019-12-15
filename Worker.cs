using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Multitasking_ThreadPool
{
    internal class Worker
    {
        private static Process _workerProcess;

        public Worker()
        {
            _workerProcess = new Process
            {
                StartInfo =
                {
                    FileName =
                        "C:\\Users\\rslow\\Downloads\\Windows_Multitasking_TASKS-master\\Windows_Multitasking_TASKS-master\\ImageWorker\\bin\\Debug\\ImageWorker.exe",
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            _workerProcess.Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Write(string message)
        {
            using (var sw = File.AppendText(".\\log.txt"))
            {
                sw.WriteLine("Write");
                sw.WriteLine(message);
            }

            _workerProcess.StandardInput.WriteLine(message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string Read()
        {
            var ret = _workerProcess.StandardOutput.ReadLine();
            using (var sw = File.AppendText(".\\log.txt"))
            {
                sw.WriteLine("Read");
                sw.WriteLine(ret);
            }
            return ret;
        }

        public void Quit()
        {
            try
            {
                _workerProcess.Kill();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}