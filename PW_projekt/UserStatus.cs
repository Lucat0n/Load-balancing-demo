using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW_projekt
{
    class UserStatus
    {
        private int clientID;
        private int fileCount;
        private List<File> userFiles;
        private List<int> elapsedAuctionCountList;
        private TimeSpan elapsedTime;
        private TimeSpan lastWonAuctionTime;

        public UserStatus(int clientID, int fileCount, TimeSpan elapsedTime, TimeSpan lastWonAuctionTime)
        {
            this.clientID = clientID;
            this.FileCount = fileCount;
            this.elapsedTime = elapsedTime;
            this.lastWonAuctionTime = lastWonAuctionTime;
            ElapsedAuctionCountList = new List<int>();
        }

        public int ClientID { get => clientID; }
        public TimeSpan ElapsedTime { get => elapsedTime; set => elapsedTime = value; }
        public TimeSpan LastWonAuction { get => lastWonAuctionTime; set => lastWonAuctionTime = value; }
        public int WonOverLast20Auctions { get => ElapsedAuctionCountList.Where(x => x < 20).Count(); }
        public int FileCount { get => fileCount; set => fileCount = value; }
        public String LastWonAuctionTimeStr { get => ElapsedAuctionCountList.Count > 0 ? lastWonAuctionTime.ToString() : "-"; }
        public int WonAuctionCount { get => ElapsedAuctionCountList.Count; }
        public List<File> UserFiles { get => userFiles; set => userFiles = value; }
        public string FilesSizes { get => AllFileSizesToString(); }
        public List<int> ElapsedAuctionCountList { get => elapsedAuctionCountList; set => elapsedAuctionCountList = value; }

        public void TriggerWonAuction()
        {
            ElapsedAuctionCountList.Add(0);
        }

        public void UpdateAuctionCountList()
        {
            if(ElapsedAuctionCountList.Count > 0)
            {
                for(int i = 0; i < ElapsedAuctionCountList.Count; i++)
                {
                    ElapsedAuctionCountList[i] += 1;
                }
            }
        }

        public int GetWonAmountDuringNTimes(int n)
        {
            return ElapsedAuctionCountList.Where(x => x < n).Count();
        }

        private string AllFileSizesToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach(File file in userFiles)
            {
                sb.Append(file.FileSize.ToString("n2"));
                sb.Append("MB");
                if(++i < userFiles.Count)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
    }
}
