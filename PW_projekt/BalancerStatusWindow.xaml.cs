using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace PW_projekt
{
    /// <summary>
    /// Logika interakcji dla klasy BalancerStatusWindow.xaml
    /// </summary>
    public partial class BalancerStatusWindow : Window
    {
        public bool isAuctionInProgress;
        private int waiting_users_count = 0;
        private Dictionary<int, List<File>> usersDict;
        private List<UserStatus> usersList;
        private DispatcherTimer dispatcherTimer;
        private ResourceStatusWindow resourceStatusWindow;

        List<UserStatus> UsersStatList { set; get; }

        public BalancerStatusWindow(ResourceStatusWindow resourceStatusWindow)
        {
            this.resourceStatusWindow = resourceStatusWindow;
            usersDict = new Dictionary<int, List<File>>();
            usersList = new List<UserStatus>();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += Timer_tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            InitializeComponent();
        }

        public void AddClient(int clientID, List<File> files)
        {
            if (usersDict.ContainsKey(clientID))
            {
                usersDict[clientID].AddRange(files);
                int index = usersList.FindIndex(x => x.ClientID == clientID);
                usersList[index].FileCount += files.Count;
                var user_stats = usersList.Where(x => x.ClientID == clientID)
                    .Select(x => (x.FileCount, x.ElapsedTime)).First();
                var count = usersList[index].FileCount;
                usersList[index].UserFiles = usersDict[clientID];
            }
            else
            {
                waiting_users_count += 1;
                Queue_Count_Label.Content = waiting_users_count.ToString();
                usersDict[clientID] = new List<File>();
                usersDict[clientID].AddRange(files);
                UserStatus userStat = new UserStatus(clientID, files.Count, TimeSpan.Zero, TimeSpan.Zero);
                userStat.UserFiles = files;
                usersList.Add(userStat);
            }
            RefreshClientsList();
            if (resourceStatusWindow.GetAvailableThreads() > 0)
                RunAuction();
        }

        private void RunAuction()
        {
            this.isAuctionInProgress = true;
            int free_threads = resourceStatusWindow.GetAvailableThreads();
            if (free_threads <= 0)
                return;
            List<(double, File, int)> weights = new List<(double, File, int)>();
            
            foreach(KeyValuePair<int,List<File>> entry in usersDict)
            {
                UserStatus userStatus = usersList.Where(x => x.ClientID == entry.Key).First();
                foreach(File file in entry.Value)
                {
                    double weight = (userStatus.GetWonAmountDuringNTimes(waiting_users_count) + 1) / (double)(waiting_users_count+1) * ((userStatus.ElapsedTime.Subtract(userStatus.LastWonAuction).TotalMilliseconds / 1000f + 1) / Math.Pow((userStatus.ElapsedTime.TotalMilliseconds / 1000f + 1), 2)) * file.FileSize;
                    weights.Add((weight, file, userStatus.ClientID));
                }
                userStatus.UpdateAuctionCountList();
            }
            var top_n = weights.OrderBy(x => x.Item1).Take(free_threads);
            foreach(var element in top_n)
            {
                usersDict[element.Item3].Remove(element.Item2);
                UserStatus temp = usersList.Where(x => x.ClientID == element.Item3).First();
                temp.UserFiles.Remove(element.Item2);
                temp.FileCount--;
                temp.TriggerWonAuction();
                if(temp.FileCount == 0)
                {
                    usersDict.Remove(temp.ClientID);
                    usersList.Remove(temp);
                    waiting_users_count -= 1;
                    Queue_Count_Label.Content = waiting_users_count.ToString();
                }
                resourceStatusWindow.StartUploadingThread(element.Item2);
                AuctionLog.AppendText("Użytkownik o ID: " + element.Item3 + " wygrał z plikiem " + element.Item2.FileSize.ToString("n2") + "MB\r");
            }
            RefreshClientsList();
            Free_Threads_Label.Content = resourceStatusWindow.GetAvailableThreads().ToString();
            this.isAuctionInProgress = false;
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            RunAuction();
            for (int i = 0; i < usersList.Count; i++)
            {
                usersList[i].ElapsedTime = usersList[i].ElapsedTime.Add(TimeSpan.FromSeconds(1));
                if (usersList[i].WonAuctionCount > 0)
                    usersList[i].LastWonAuction = usersList[i].LastWonAuction.Add(TimeSpan.FromSeconds(1));
            }
            RefreshClientsList();
        }

        private void RefreshClientsList()
        {
            Clients_List.ItemsSource = null;
            Clients_List.ItemsSource = usersList;
        }

    }
}
