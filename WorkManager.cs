using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Multitasking_ThreadPool
{
    internal class WorkManager
    {
        private static WorkManager _instance;
        private static Worker _worker;

        private static readonly Mutex Mutex = new Mutex();

        private readonly List<string> _logs;
        private readonly FileWatcher _watcher;

        private WorkManager()
        {
            _worker = new Worker();
            _watcher = new FileWatcher();
            _logs = new List<string>();
        }

        public static WorkManager GetInstance()
        {
            return _instance ?? (_instance = new WorkManager());
        }

        public void StartWatcher(string path)
        {
            ProcessExistingFiles(path);
            _watcher.Start(path);
        }

        private void ProcessExistingFiles(string path)
        {
            var files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            foreach (var file in files) ProcessFile(file);
        }

        public void AbortWatcher()
        {
            _watcher?.Abort();
        }

        public string ReadFromWorker()
        {
            return _worker.Read();
        }

        public void WriteToWorker(string message)
        {
            _worker.Write(message);
        }

        public void Quit()
        {
            _worker.Quit();
        }

        public void SetOutputDir(string outDir)
        {
            Mutex.WaitOne();
            _logs.Insert(0, "Setting out dir: " + outDir);
            WriteToWorker("set_output_dir");
            WriteToWorker(outDir);
            Mutex.ReleaseMutex();
        }

        public void SetImageSize(string height, string width)
        {
            Mutex.WaitOne();
            _logs.Insert(0, "Setting height: " + height);
            WriteToWorker("set_height");
            WriteToWorker(height);
            Mutex.ReleaseMutex();

            Mutex.WaitOne();
            _logs.Insert(0, "Setting width: " + width);
            WriteToWorker("set_width");
            WriteToWorker(width);
            Mutex.ReleaseMutex();
        }

        public List<string> GetQueuedFiles()
        {
            Mutex.WaitOne();
            WriteToWorker("get_queued_files");
            var files = ReadFromWorker();
            Mutex.ReleaseMutex();
            if (files == "Null" || files == null)
                return new List<string>();
            return files.Split(',').ToList();
        }

        public List<string> GetLogs()
        {
            return _logs;
        }

        public string GetCurrentFiles()
        {
            Mutex.WaitOne();
            WriteToWorker("get_current_files");
            var files = ReadFromWorker();
            Mutex.ReleaseMutex();
            if (files == "Null" || files == null)
                return "No files";
            return files;
        }

        public string GetProgress()
        {
            Mutex.WaitOne();
            WriteToWorker("get_progress");
            var progress = ReadFromWorker();
            Mutex.ReleaseMutex();
            return progress;
        }

        public void ProcessFile(string path)
        {
            _logs.Insert(0, "Processing file " + path);
            Mutex.WaitOne();
            GetInstance().WriteToWorker("add");
            GetInstance().WriteToWorker(path);
            Mutex.ReleaseMutex();
        }
    }
}