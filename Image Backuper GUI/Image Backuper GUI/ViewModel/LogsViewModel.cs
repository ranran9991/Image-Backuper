using Image_Backuper_GUI.Message;
using Image_Backuper_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Backuper_GUI.ViewModel
{
    public class LogsViewModel
    {
        public LogsViewModel()
        {
            model = new LogsModel();
        }

        private LogsModel model { get; set; }

        public ObservableCollection<MessageRecievedEventArgs> logs
        {
            get { return model.logs; }
        }
    }
}
