using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image_Backuper_GUI.Client;

namespace Image_Backuper_GUI.Model
{
    class MainModel
    {
        public GUIClient client { get; set; }

        public MainModel()
        {
            client = GUIClient.Instance;
        }

        public bool connected { get { return client.connected; } }
    }
}
