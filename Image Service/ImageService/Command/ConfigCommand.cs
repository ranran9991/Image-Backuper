using ImageService.Commands;
using Infrastructure;
using ImageService.Model;
using ImageService.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Command
{
    class ConfigCommand : ICommand
    {
        ImageServer server;

        public ConfigCommand(ImageServer serv)
        {
            server = serv;
        }
        public string Execute(string[] args, out bool result)
        {
           
            JObject JsonConfig = new JObject();
            JArray pathJSON = new JArray();
            foreach(string path in server.pathList)
            {
                pathJSON.Add(path);
            }
            JsonConfig["Handler"] = pathJSON;
            JsonConfig["OutputDir"] = ConfigurationManager.AppSettings["OutputDir"];
            JsonConfig["SourceName"] = ConfigurationManager.AppSettings["SourceName"];
            JsonConfig["LogName"] = ConfigurationManager.AppSettings["LogName"];
            JsonConfig["ThumbnailSize"] = ConfigurationManager.AppSettings["ThumbnailSize"];
            string[] cmndArgs = { JsonConfig.ToString() };
            CommandRecievedEventArgs cmndRecieved = new CommandRecievedEventArgs((int)CommandEnum.ConfigCommand, cmndArgs, null);
            result = true;
            return cmndRecieved.ToJSON();
        
        }
    }
}
