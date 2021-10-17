using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
    /// Logika interakcji dla klasy ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private List<File> files;
        private int clientID;
        private BalancerStatusWindow balancer_handle;
        public ClientWindow(int clientID, BalancerStatusWindow balancer_handle)
        {
            this.clientID = clientID;
            this.balancer_handle = balancer_handle;
            files = new List<File>();
            InitializeComponent();
            ((INotifyCollectionChanged)Files_To_Upload_View.Items).CollectionChanged += Handle_list_change;
        }

        private void Upload_All_Button_Click(object sender, RoutedEventArgs e)
        {
            Files_To_Upload_View.Items.Clear();
            balancer_handle.AddClient(clientID, files);
            files.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Error_Label.Content = "";
            double file_size;
            bool is_double = Double.TryParse(FileSizeBox.Text, out file_size);
            if (!is_double || file_size < 0)
            {
                Error_Label.Content = "Wrong size";
                return;
            }
            if (!String.IsNullOrEmpty(FileNameBox.Text) && !String.IsNullOrEmpty(FileSizeBox.Text))
            {
                File file = new File(FileNameBox.Text, float.Parse(FileSizeBox.Text));
                files.Add(file);
                Files_To_Upload_View.Items.Add(file);
            }
        }

        private void Handle_list_change(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(Files_To_Upload_View.Items.Count > 0)
                Upload_All_Button.IsEnabled = true;
            else
                Upload_All_Button.IsEnabled = false;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Files_To_Upload_View.Items.Clear();

            files.Clear();
        }
    }
}
