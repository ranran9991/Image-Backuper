using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Image_Backuper_GUI.Config
{
    class ConfigData
    {
        public ConfigData(List<string> Handler, string OutputDir, string SourceName, string LogName, int ThumbnailSize)
        {
            handlersLock = new object();
            dataLock = new object();

            try { this.Handler = new ObservableCollection<string>(Handler); }
            catch { }
            Data = new ObservableCollection<DataMember>();
            Data.Add(new DataMember("OutputDir", OutputDir));
            Data.Add(new DataMember("SourceName", SourceName));
            Data.Add(new DataMember("LogName", LogName));
            Data.Add(new DataMember("ThumbnailSize", ThumbnailSize.ToString()));
        }

        public void Set(ConfigData config)
        {
            foreach (string handler in config.Handler)
            {
                this.Handler.Add(handler);
            }

            Data.Clear();
            foreach (DataMember data in config.Data)
            {
                this.Data.Add(data);
            }
        }

        private readonly object handlersLock;
        private readonly object dataLock;

        private ObservableCollection<string> _handlers;
        private ObservableCollection<DataMember> _data;

        public ObservableCollection<string> Handler
        {
            get
            {
                return _handlers;
            }
            set
            {
                _handlers = value;
                BindingOperations.EnableCollectionSynchronization(_handlers, handlersLock);
            }
        }

        public ObservableCollection<DataMember> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                BindingOperations.EnableCollectionSynchronization(_data, dataLock);
            }
        }

        public static ConfigData FromJSON(string str)
        {
            JObject cnJson = JObject.Parse(str);
            JArray JHandler = (JArray)cnJson["Handler"];
            List<string> Handler = new List<string>();
            foreach (JToken obj in JHandler.Children())
            {
                Handler.Add(obj.ToString());
            }
            string OutputDir = (string)cnJson["OutputDir"];
            string SourceName = (string)cnJson["SourceName"];
            string LogName = (string)cnJson["LogName"];
            int ThumbnailSize = int.Parse((string)cnJson["ThumbnailSize"]);
            return new ConfigData(Handler, OutputDir, SourceName, LogName, ThumbnailSize);
        }
    }
}
