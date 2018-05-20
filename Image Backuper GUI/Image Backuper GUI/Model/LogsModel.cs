using Image_Backuper_GUI.Client;
using Image_Backuper_GUI.Command;
using Image_Backuper_GUI.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Image_Backuper_GUI.Model
{
    class LogsModel
    {
        public GUIClient client { get; set; }

        public LogsModel()
        {
            client = GUIClient.Instance;
            logsLock = new object();
            try { logs = new ObservableCollection<MessageRecievedEventArgs>(client.GetLogs()); }
            catch { }
            client.CommandReceived += AddLog;
            client.Register();
        }

        private readonly object logsLock;

        private ObservableCollection<MessageRecievedEventArgs> _logs;

        public ObservableCollection<MessageRecievedEventArgs> logs
        {
            get
            {
                return _logs;
            }
            set
            {
                _logs = value;
                BindingOperations.EnableCollectionSynchronization(_logs, logsLock);
            }
        }

        public void AddLog(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.LogCommand)
            {
                logs.Add(MessageRecievedEventArgs.FromJSON(e.Args[0]));
            }
        }
    }
}
