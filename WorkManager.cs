using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Multitasking_ThreadPool
{
    class WorkManager
    {
        private static WorkManager instance = null;
        private static Worker worker;
        private FileWatcher watcher;

        private List<string> logs;

        private static Mutex mutex = new Mutex();

        public static WorkManager getInstance()
        {
            if (instance == null)
                instance = new WorkManager();
            return instance;
        }

        private WorkManager()
        {
            worker = new Worker();
            watcher = new FileWatcher();
            logs = new List<string>();
        }

        public void StartWatcher(string path)
        {
            ProcessExistingFiles(path);
            watcher.Start(path);
        }

        private void ProcessExistingFiles(string path)
        {
            string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                ProcessFile(file);
            }
        }

        public void AbortWatcher()
        {
            watcher?.Abort();
        }

        public string ReadFromWorker()
        {
            return worker.Read();
        }

        public void WriteToWorker(string message)
        {
            worker.Write(message);
        }

        public void Quit()
        {
            worker.Quit();
        }

        public void SetOutputDir(string outDir)
        {
            mutex.WaitOne();
            logs.Insert(0,"Setting out dir: " + outDir);
            WriteToWorker("set_output_dir");
            WriteToWorker(outDir);
            SyncWorker();
        }

        private void SyncWorker()
        {
            WriteToWorker("SYNC");
            mutex.ReleaseMutex();
        }

        public void SetImageSize(string height, string width)
        {
            mutex.WaitOne();
            logs.Insert(0, "Setting height: " + height);
            WriteToWorker("set_height");
            WriteToWorker(height);
            SyncWorker();

            mutex.WaitOne();
            logs.Insert(0,"Setting width: " + width);
            WriteToWorker("set_width");
            WriteToWorker(width);
            SyncWorker();
        }

        public List<string> GetQueuedFiles()
        {
            mutex.WaitOne();
            WriteToWorker("get_queued_files");
            var files = ReadFromWorker();
            SyncWorker();
            if (files == "Null" || files == null)
                return new List<string>();
            return files.Split(',').ToList();
        }

        public List<string> GetLogs()
        {
            return logs;
        }

        public string GetCurrentFiles()
        {
            mutex.WaitOne();
            WriteToWorker("get_current_files");
            var files = ReadFromWorker();
            SyncWorker();
            if (files == "Null" || files == null)
                return "No files";
            return files;
        }

        public string GetProgress()
        {
            mutex.WaitOne();
            WriteToWorker("get_progress");
            var progress = ReadFromWorker();
            SyncWorker();
            return progress;
        }

        public void ProcessFile(string path)
        {
            logs.Insert(0,"Processing file " + path);
            mutex.WaitOne();
            WorkManager.getInstance().WriteToWorker("add");
            WorkManager.getInstance().WriteToWorker(path);
            SyncWorker();
        }
    }
}
