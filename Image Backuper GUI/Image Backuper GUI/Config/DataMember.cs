using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Backuper_GUI.Config
{
    public class DataMember
    {
        public string key { get; set; }
        public string value { get; set; }

        public DataMember(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
