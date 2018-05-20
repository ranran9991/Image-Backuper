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

            try { this.Handler = new ObservableCollection<string>(Handler); }
            catch { }
            this.OutputDir = OutputDir;
            this.SourceName = SourceName;
            this.LogName = LogName;
            this.ThumbnailSize = ThumbnailSize;
        }

        private readonly object handlersLock;

        private ObservableCollection<string> _handlers;

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
        
        public string OutputDir { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }

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
