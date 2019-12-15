using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AForge.Imaging.Filters;

namespace ImageWorker
{
    class ImageWorker
    {
        public static List<string> QueuedFiles = new List<string>();
        public static List<string> CurrentFiles = new List<string>();
        public static List<string> LeftFiles = new List<string>();
        public static string OutputFolder { get; set; } = null;
        public static int TotalFiles { get; set; } = 0;
        public static int FilesProcessed { get; set; } = 0;

        public static int Height { get; set; } = 3000;
        public static int Width { get; set; } = 3000;

        public static int MaxFiles { get; set; } = 2;
        private static Semaphore semaphore = new Semaphore(MaxFiles, MaxFiles);


        public static void QueueFiles(params string[] path)
        {
            TotalFiles += path.Length;

            foreach (var file in path)
            {
                QueuedFiles.Add(file);
                LeftFiles.Add(file);
            }
        }

        public static void ProcessQueue()
        {
            for (int entry = QueuedFiles.Count - 1; entry >= 0; entry--)
            {
                var queuedFile = QueuedFiles[entry];
                QueuedFiles.Remove(queuedFile);
                Task.Factory.StartNew(() => ProcessFile(queuedFile));
            }
        }

        public static void ProcessFile(string filePath)
        {
            try
            {
                semaphore.WaitOne();
                var filename = filePath.Split('\\').Last();
                LeftFiles.Remove(filePath);
                CurrentFiles.Add(filename);
                ResizeBicubic filter = new ResizeBicubic(Width, Height);
                Bitmap image = AForge.Imaging.Image.FromFile(filePath);
                Bitmap newImage = filter.Apply(image);

                newImage.Save(OutputFolder + "\\" + filename);
                FilesProcessed++;
                CurrentFiles.Remove(filename);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                QueuedFiles.Add(filePath);
                LeftFiles.Add(filePath);
            }
            semaphore.Release();

        }

    }
}
