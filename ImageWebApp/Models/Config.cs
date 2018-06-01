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
    }
}