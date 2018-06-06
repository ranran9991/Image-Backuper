using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageWebApp.Models
{
    public class Thumbnail
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Thumbnail(string name, string path, int month, int year)
        {
            Name = name;
            Path = path;
            Month = month;
            Year = year;
        }

        public JObject ToJSON()
        {
            JObject obj = new JObject();
            obj["Name"] = Name;
            obj["Path"] = Path;
            obj["Month"] = Month;
            obj["Year"] = Year;

            return obj;
        }
    }
}