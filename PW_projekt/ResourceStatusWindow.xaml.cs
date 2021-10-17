using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PW_projekt
{
    /// <summary>
    /// Logika interakcji dla klasy ResourceStatusWindow.xaml
    /// </summary>
    public partial class ResourceStatusWindow : Window
    {
        private const float UPLOAD_RATE = 0.2f; //w MB/s
        private const int THREAD_COUNT = 5;

        public bool AreAllDisksOccupied { get => diskOccupationArray.All(x => x); }

        private bool[] diskOccupationArray = { false, false, false, false, false};

        public ResourceStatusWindow()
        {
            ThreadPool.SetMaxThreads(THREAD_COUNT,0);
            InitializeComponent();
            InitLabels();
        }

        public int GetAvailableThreads()
        {
            int available_worker = 0;
            int available_io = 0;
            ThreadPool.GetAvailableThreads(out available_worker, out available_io);

            int max_worker = 0;
            int max_io = 0;
            ThreadPool.GetMaxThreads(out max_worker, out max_io);

            return THREAD_COUNT - (max_worker - available_worker);

        }

        public void StartUploadingThread(File file)
        {
            if(GetAvailableThreads() > 0)
                ThreadPool.QueueUserWorkItem(UploadFile, new object[] { file });
        }

        private void UploadFile(object state)
        {
            object[] arr = state as object[];
            File file = (File)arr[0];
            float progress = 0.0f;
            int id = GetAvailableDiskID();
            diskOccupationArray[id] = true;
            var DiskLabel = this.Dispatcher.Invoke(() => { return (Label)FindName("OccupiedList" + id); });
            var DiskProgress = this.Dispatcher.Invoke(() => { return (ProgressBar)FindName("DiskProgressBar" + id); });
            var DoneLabel = this.Dispatcher.Invoke(() => { return (Label)FindName("DoneLabel" + id); });
            this.Dispatcher.Invoke(() => { DoneLabel.Content = ""; });
            Dispatcher.Invoke(() => { DiskLabel.Content = "Przesyłanie\n" + file.FileName; });
            Dispatcher.Invoke(() =>
            {
                DiskProgress.Minimum = 0;
            });
            Dispatcher.Invoke(() =>
            {
                DiskProgress.Maximum = file.FileSize;
            });
            while (!IsUploadDone(file, progress))
            {
                Thread.Sleep(500);
                progress += UPLOAD_RATE;
                Dispatcher.Invoke(() =>
                {
                    DiskProgress.Value = progress;
                });
            }
            diskOccupationArray[id] = false;
            Dispatcher.Invoke(() => { DoneLabel.Content = "Przesłano \n" + file.FileName; });
        }

        private int GetAvailableDiskID()
        {
            for(int i = 0; i < diskOccupationArray.Length; i++)
                if (diskOccupationArray[i] == false)
                    return i;
            return -1;
        }

        /*
         * returns 0 if finished uploading, 1 otherwise
         */
        private bool IsUploadDone(File file, float progress)
        {
            if (file.FileSize > progress)
                return false;
            return true;
        }

        private void InitLabels()
        {
            OccupiedList0.Content = "-";
            OccupiedList1.Content = "-";
            OccupiedList2.Content = "-";
            OccupiedList3.Content = "-";
            OccupiedList4.Content = "-";
        }

    }
}
