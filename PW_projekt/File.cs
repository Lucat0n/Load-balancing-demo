using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW_projekt
{
    public class File
    {
        public File() { }

        public File(String name, float size)
        {
            this.name = name;
            this.size = size;
        }

        public string FileName { get => name;}
        public float FileSize { get => size;}

        private String name;
        private float size;
    }
}
