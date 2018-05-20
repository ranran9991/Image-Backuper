using Image_Backuper_GUI.Model;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Backuper_GUI.ViewModel
{
    class MainViewModel
    {
        public MainViewModel()
        {
            model = new MainModel();
            connected = model.connected;
        }

        private MainModel model { get; set; }

        private bool connected { get; set; }

        public Brush ConnectionColor
        {
            get
            {
                if (connected)
                    return Brushes.LightGreen;
                return Brushes.Gray;
            }
        }
    }
}
