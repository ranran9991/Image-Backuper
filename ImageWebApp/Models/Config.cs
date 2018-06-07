using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageWebApp.Models
{
    public class Config
    {
        public string OutputDir { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }

        public Config(string OutputDir, string SourceName, string LogName, int ThumbnailSize)
        {
            this.OutputDir = OutputDir;
            this.SourceName = SourceName;
            this.LogName = LogName;
            this.ThumbnailSize = ThumbnailSize;
        }

        public static Config FromJSON(string str)
        {
            JObject cnJson = JObject.Parse(str);
            string OutputDir = (string)cnJson["OutputDir"];
            string SourceName = (string)cnJson["SourceName"];
            string LogName = (string)cnJson["LogName"];
            int ThumbnailSize = int.Parse((string)cnJson["ThumbnailSize"]);
            return new Config(OutputDir, SourceName, LogName, ThumbnailSize);
        }
    }
}