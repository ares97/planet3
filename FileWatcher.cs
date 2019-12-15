using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace Multitasking_ThreadPool
{
    internal class FileWatcher
    {
        private Thread _watcherThread;

        public void Start(string path)
        {
            Abort();
            _watcherThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Run(path);
            });
            _watcherThread.Start();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Run(string path)
        {
            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = path;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.png";
                watcher.Created += OnNewFile;
                watcher.EnableRaisingEvents = true;
                while (true)
                {
                }
            }
        }



        private static void OnNewFile(object source, FileSystemEventArgs e)
        {
            WorkManager.GetInstance().ProcessFile(e.FullPath);
        }

        public void Abort()
        {
            _watcherThread?.Abort();
        }
    }
}