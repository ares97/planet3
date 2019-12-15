using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace Multitasking_ThreadPool
{
    class FileWatcher
    {
        private Thread watcherThread = null;

        public void Start(string path)
        {
            Abort();
            watcherThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Run(path);
            });
            watcherThread.Start();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Run(string path)
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = path;

                watcher.NotifyFilter = NotifyFilters.FileName
                                       | NotifyFilters.DirectoryName;

                watcher.Filter = "*.png";

                watcher.Changed += OnChanged;
                watcher.Created += OnNewFile;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                watcher.EnableRaisingEvents = true;

                while (true);
            }
        }


        private static void OnChanged(object source, FileSystemEventArgs e) { }

        private static void OnNewFile(object source, FileSystemEventArgs e)
        {
            WorkManager.getInstance().ProcessFile(e.FullPath);
        }

        private static void OnRenamed(object source, RenamedEventArgs e) { }

        public void Abort()
        {
            watcherThread?.Abort();
        }


    }
}
