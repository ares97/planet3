using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Multitasking_ThreadPool
{
    public partial class MainWindow : Window
    {
        ViewModel _viewModel = new ViewModel("select path", "select path");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            new Thread(new ThreadStart(ServeGui)).Start();
        }

        void ServeGui()
        {
            while (true)
            {
                Thread.Sleep(100);
                var currentFiles = WorkManager.getInstance().GetCurrentFiles();
                currentFiles_Label.Dispatcher?.BeginInvoke((Action)(() => currentFiles_Label.Content = currentFiles));

                var queuedFiles = WorkManager.getInstance().GetQueuedFiles();
                filesQueueListBox.Dispatcher?.BeginInvoke((Action)(() => filesQueueListBox.ItemsSource = queuedFiles));
                filesQueueListBox.Dispatcher?.BeginInvoke((Action)(() => CollectionViewSource.GetDefaultView(filesQueueListBox.ItemsSource).Refresh()));

                var logs = WorkManager.getInstance().GetLogs();
                //logs.Reverse();
                logsListBox.Dispatcher?.BeginInvoke((Action)(() => logsListBox.ItemsSource = logs));
                logsListBox.Dispatcher?.BeginInvoke((Action)(() => CollectionViewSource.GetDefaultView(logsListBox.ItemsSource).Refresh()));

                var progress = WorkManager.getInstance().GetProgress();
                try
                {
                    progress = ((int) (float.Parse(progress) * 100)).ToString();
                    currentProgress_Label.Dispatcher?.BeginInvoke((Action)(() => currentProgress_Label.Content = progress));
                } catch(Exception) { }
            }
        }

        private void inputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ((ViewModel)DataContext).inputPath = dialog.SelectedPath;
                    observedDir_Label.GetBindingExpression(ContentProperty)?.UpdateTarget();
                    WorkManager.getInstance().AbortWatcher();
                }
            }
        }

        private void outputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ((ViewModel)DataContext).outputPath = dialog.SelectedPath;
                    outputDir_Label.GetBindingExpression(ContentProperty)?.UpdateTarget();
                    var outDir = _viewModel.outputPath;
                    WorkManager.getInstance().SetOutputDir(outDir);
                }
            }
        }

        private void startObserving_Click(object sender, RoutedEventArgs e)
        {
            string path = _viewModel.inputPath;
            WorkManager.getInstance().StartWatcher(path);
        }

        private void SetNewSize_Click(object sender, RoutedEventArgs e)
        {
            WorkManager.getInstance().SetImageSize(ImageHeight.Text, ImageWidth.Text);
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            WorkManager.getInstance().Quit();
        }
    }
}
