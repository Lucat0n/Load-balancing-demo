using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW_projekt
{
    class User
    {
        public User(int ID, List<File> files)
        {
            this.ID = ID;
            this.filesToUpload = files;
            initialTime = TimeSpan.Zero;
        }

        int ID;
        List<File> filesToUpload;
        TimeSpan initialTime;
    }
}
