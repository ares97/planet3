using System;
using System.Drawing;
using Microsoft.Win32;

namespace ImageWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var command = Console.ReadLine();
                string arg;
                switch (command)
                {
                    case "exit":
                        return;
                    case "add":
                        arg = Console.ReadLine();
                        ImageWorker.QueueFiles(arg);
                        break;
                    case "set_input_dir":
                        arg = Console.ReadLine();
                        ImageWorker.QueueFiles(arg);
                        break;
                    case "set_output_dir":
                        arg = Console.ReadLine();
                        ImageWorker.OutputFolder = arg;
                        break;
                    case "set_height":
                        arg = Console.ReadLine();
                        if (int.TryParse(arg, out int height))
                        {
                            ImageWorker.Height = height;
                        }
                        break;
                    case "set_width":
                        arg = Console.ReadLine();
                        if (int.TryParse(arg, out int width))
                        {
                            ImageWorker.Width = width;
                        }

                        break;
                    case "get_current_files":
                        arg = String.Join(", ", ImageWorker.CurrentFiles.ToArray());
                        if (arg != "")
                            Console.WriteLine(arg);
                        else
                            Console.WriteLine("Null");
                        break;
                    case "get_queued_files":
                        arg = String.Join(", ", ImageWorker.LeftFiles.ToArray());
                        if (arg != "")
                            Console.WriteLine(arg);
                        else
                            Console.WriteLine("Null");
                        break;
                    case "get_progress":
                        float progress = (float) ImageWorker.FilesProcessed / Math.Max(1, ImageWorker.TotalFiles);
                        arg = progress.ToString();
                        Console.WriteLine(arg);
                        break;
                }
                ImageWorker.ProcessQueue();
            }
        }
    }
}