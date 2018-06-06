using ImageWebApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure; 

namespace ImageWebApp.Controllers
{
    public class ImageController : Controller
    {
        public static ImageModel imageModel = new ImageModel();
        // GET: Image
        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Config()
        {
            Config config = new Config
            {
                OutputDir = "OutputDir test",
                SourceName = "SourceName test",
                LogName = "LogName test",
                ThumbnailSize = 120
            };

            return View(config);
        }
        public ActionResult Log()
        {
            Log log = new Log();
            log.log = new List<MessageRecievedEventArgs>{
                new MessageRecievedEventArgs("Hello", MessageTypeEnum.INFO),
                new MessageRecievedEventArgs("GoodBye", MessageTypeEnum.FAIL)
            };

            return View(log);
        }
        public ActionResult Image()
        {
            return View();
        }

        [HttpPost]
        public JObject GetDetails()
        {
            string filePath = Server.MapPath("/App_Data/details.txt");
            StreamReader reader = new StreamReader(filePath);
            string[] firstPerson = reader.ReadLine().Split(' ');
            string[] secondPerson = reader.ReadLine().Split(' ');
            reader.Close();

            JObject detailsJson = new JObject();

            JObject first = new JObject
            {
                ["FirstName"] = firstPerson[0],
                ["LastName"] = firstPerson[1],
                ["ID"] = firstPerson[2]
            };

            JObject second = new JObject
            {
                ["FirstName"] = secondPerson[0],
                ["LastName"] = secondPerson[1],
                ["ID"] = secondPerson[2]
            };

            detailsJson["First"] = first;
            detailsJson["Second"] = second;

            return detailsJson;
        }

        [HttpPost]
        public JObject GetThumbnails()
        {
            JObject obj = JObject.Parse(imageModel.ThumbnailsToJSON());
            return obj;
        }

        [HttpPost]
        public string GetRealImagePath(string thumbPath)
        {
            return imageModel.GetRealImagePath(thumbPath);
        }

        [HttpPost]
        public void RemovePicture(string thumbPath)
        {
            imageModel.RemovePicture(thumbPath);
        }
    }

}