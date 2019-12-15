using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Multitasking_ThreadPool
{
    class Worker
    {
        private static Process workerProcess;
        public Worker()
        {
            workerProcess = new Process();

            workerProcess.StartInfo.FileName = "C:\\Users\\rslow\\Downloads\\Windows_Multitasking_TASKS-master\\Windows_Multitasking_TASKS-master\\ImageWorker\\obj\\Debug\\ImageWorker.exe";

            workerProcess.StartInfo.CreateNoWindow = false;
            workerProcess.StartInfo.UseShellExecute = false;

            workerProcess.StartInfo.RedirectStandardInput = true;
            workerProcess.StartInfo.RedirectStandardOutput = true;
            workerProcess.StartInfo.RedirectStandardError = true;

            workerProcess.Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Write(string message)
        {
            using (StreamWriter sw = File.AppendText(".\\log.txt"))
            {
                sw.WriteLine("Write");
                sw.WriteLine(message);
            }
            workerProcess.StandardInput.WriteLine(message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string Read()
        {
            var ret = workerProcess.StandardOutput.ReadLine();
            using (StreamWriter sw = File.AppendText(".\\log.txt"))
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
                workerProcess.Kill();
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
