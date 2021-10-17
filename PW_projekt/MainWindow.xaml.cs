using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PW_projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int userIDCounter = 0;
        private Random rand;
        private readonly BalancerStatusWindow bal_window;
        private readonly ResourceStatusWindow res_window;
        public MainWindow()
        {
            rand = new Random();
            res_window = new ResourceStatusWindow();
            bal_window = new BalancerStatusWindow(res_window);
            bal_window.InitializeComponent();
            res_window.InitializeComponent();
            InitializeComponent();
        }

        private void Bal_Button_Click(object sender, RoutedEventArgs e)
        {
            if (bal_window.Visibility == Visibility.Hidden || bal_window.Visibility == Visibility.Collapsed)
                bal_window.Show();
            else if (bal_window.Visibility == Visibility.Visible)
                bal_window.Hide();
        }

        private void Res_Button_Click(object sender, RoutedEventArgs e)
        {
            if (res_window.Visibility == Visibility.Hidden || res_window.Visibility == Visibility.Collapsed)
                res_window.Show();
            else if(res_window.Visibility == Visibility.Visible)
                res_window.Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            bal_window.Close();
            res_window.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow client_window = new ClientWindow(userIDCounter, bal_window);
            userIDCounter++;
            client_window.InitializeComponent();
            client_window.Show();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            //ClientWindow client_window = new ClientWindow(userIDCounter, bal_window);
            List<File> files = new List<File>();
            int files_to_gen = rand.Next(1, 8);
            for(int i=0; i < files_to_gen; i++)
            {
                float size = (float)rand.NextDouble()*10f;
                files.Add(new File("File-" + userIDCounter.ToString() + "-" + i.ToString(),size));
            }
            bal_window.AddClient(userIDCounter, files); ;
            userIDCounter++;
            //client_window.InitializeComponent();
            //client_window.Show();
        }
    }
}
