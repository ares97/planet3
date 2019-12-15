using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

namespace Multitasking_ThreadPool
{
    public partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel = new ViewModel("select path", "select path");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            //new Thread(ServeGui).Start();
        }

        private void ServeGui()
        {
            while (true)
            {
                Thread.Sleep(100);
                var currentFiles = WorkManager.GetInstance().GetCurrentFiles();
                currentFiles_Label.Dispatcher?.BeginInvoke((Action) (() => currentFiles_Label.Content = currentFiles));

                var queuedFiles = WorkManager.GetInstance().GetQueuedFiles();
                filesQueueListBox.Dispatcher?.BeginInvoke((Action) (() => filesQueueListBox.ItemsSource = queuedFiles));
                filesQueueListBox.Dispatcher?.BeginInvoke((Action) (() =>
                    CollectionViewSource.GetDefaultView(filesQueueListBox.ItemsSource).Refresh()));

                var logs = WorkManager.GetInstance().GetLogs();
                logsListBox.Dispatcher?.BeginInvoke((Action) (() => logsListBox.ItemsSource = logs));
                logsListBox.Dispatcher?.BeginInvoke((Action) (() =>
                    CollectionViewSource.GetDefaultView(logsListBox.ItemsSource).Refresh()));

                var progress = WorkManager.GetInstance().GetProgress();
                try
                {
                    progress = ((int) (float.Parse(progress) * 100)).ToString();
                    currentProgress_Label.Dispatcher?.BeginInvoke((Action) (() =>
                        currentProgress_Label.Content = progress));
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void InputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                ((ViewModel) DataContext).InputPath = dialog.SelectedPath;
                observedDir_Label.GetBindingExpression(ContentProperty)?.UpdateTarget();
                WorkManager.GetInstance().AbortWatcher();
            }
        }

        private void OutputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                ((ViewModel) DataContext).OutputPath = dialog.SelectedPath;
                outputDir_Label.GetBindingExpression(ContentProperty)?.UpdateTarget();
                var outDir = _viewModel.OutputPath;
                WorkManager.GetInstance().SetOutputDir(outDir);
            }
        }

        private void StartObserving_Click(object sender, RoutedEventArgs e)
        {
            var path = _viewModel.InputPath;
            WorkManager.GetInstance().StartWatcher(path);
        }

        private void SetNewSize_Click(object sender, RoutedEventArgs e)
        {
            WorkManager.GetInstance().SetImageSize(ImageHeight.Text, ImageWidth.Text);
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            WorkManager.GetInstance().Quit();
        }
    }
}