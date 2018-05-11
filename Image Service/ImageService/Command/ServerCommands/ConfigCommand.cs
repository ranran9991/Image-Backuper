using ImageService.Commands;
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
        
        /// <summary>
        /// Creats JSON file from AppConfig and returns it
        /// </summary>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns>JSON representation of appconfig</returns>
        public string Execute(string[] args, out bool result)
        {
            try
            {
                JObject app = new JObject
                {
                    ["Handler"] = ConfigurationManager.AppSettings["Handler"],
                    ["OutputDir"] = ConfigurationManager.AppSettings["OutputDir"],
                    ["SourceName"] = ConfigurationManager.AppSettings["SourceName"],
                    ["LogName"] = ConfigurationManager.AppSettings["LogName"],
                    ["ThumbnailSize"] = ConfigurationManager.AppSettings["ThumbnailSize"]
                };

                result = true;
                return app.ToString();
            }
            catch (Exception)
            {
                result = false;
                return null;
            }
        }
    }
}
