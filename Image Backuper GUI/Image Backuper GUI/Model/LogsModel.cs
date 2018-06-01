using Image_Backuper_GUI.Client;
using Infrastructure;
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
            logs = new ObservableCollection<MessageRecievedEventArgs>();
            client.CommandReceived += HandleCommand;
            client.Register();
            client.SendCommand(new CommandRecievedEventArgs((int)CommandEnum.LogHistoryCommand, null, null));
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

        public void HandleCommand(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.LogChangedCommand)
            {
                logs.Insert(0, MessageRecievedEventArgs.FromJSON(e.Args[0]));
                return;
            }

            else if (e.CommandID == (int)CommandEnum.LogHistoryCommand)
            {
                try
                {
                    foreach (MessageRecievedEventArgs log in MessageRecievedEventArgs.LogFromJSON(e.Args[0])) { 
                        logs.Insert(0, log);
                    }
                }
                catch { }
            }
        }
    }
}
