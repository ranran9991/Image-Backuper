using Image_Backuper_GUI.Config;
using Image_Backuper_GUI.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Image_Backuper_GUI.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public SettingsViewModel()
        {
            model = new SettingsModel();
            RemoveCommand = new DelegateCommand<object>(DeleteHandler, CanDelete);
            handlerIndex = -1;
            PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                var command = this.RemoveCommand as DelegateCommand<object>;
                command.RaiseCanExecuteChanged();
            };
        }

        private SettingsModel model { get; set; }

        private int _handlerIndex;
        public int handlerIndex
        {
            get { return _handlerIndex; }
            set
            {
                _handlerIndex = value;
                OnPropertyChanged("handlerIndex");
            }
        }

        public ObservableCollection<String> directoryList
        {
            get { return model.config.Handler; }
        }

        public ObservableCollection<DataMember> data
        {
            get { return model.config.Data; }
        }

        public ICommand RemoveCommand { get; private set; }

        public void DeleteHandler(object obj)
        {
            model.DeleteHandler(directoryList[handlerIndex]);
            handlerIndex = -1;
        }

        public bool CanDelete(object obj)
        {
            return handlerIndex != -1;
        }
    }
}
