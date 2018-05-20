using Image_Backuper_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Image_Backuper_GUI.ViewModel
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            model = new SettingsModel();
        }

        private SettingsModel model { get; set; }

        public ObservableCollection<String> directoryList
        {
            get { return model.config.Handler; }
        }

        public string OutputDir
        {
            get { return model.config.OutputDir; }
        }

        public string SourceName
        {
            get { return model.config.SourceName; }
        }

        public string LogName
        {
            get { return model.config.LogName; }
        }

        public int ThumbnailSize
        {
            get { return model.config.ThumbnailSize; }
        }

        public void DeleteHandler(string dir)
        {
            model.DeleteHandler(dir);
        }
    }
}
