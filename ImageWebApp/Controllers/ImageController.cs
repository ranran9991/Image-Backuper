using ImageWebApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure;
using ImageWebApp.Client;

namespace ImageWebApp.Controllers
{
    public class ImageController : Controller
    {
        public static ImageModel imageModel = new ImageModel();
        // GET: Image
        public ActionResult Main()
        {
            ViewBag.IsConnected = WebClient.Instance.connected;
            return View();
        }

        public ActionResult Config()
        {
            return View(WebClient.Instance.config);
        }
        public ActionResult Log()
        {   
            return View(WebClient.Instance.logs);
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
            imageModel.thumbnails = imageModel.GetThumbnails();
            JObject obj = JObject.Parse(imageModel.ThumbnailsToJSON());
            return obj;
        }

        [HttpPost]
        public string GetRealImagePath(string thumbPath)
        {
            return imageModel.GetRealImagePath(thumbPath);
        }

        [HttpPost]
        public string RemovePicture(string thumbPath)
        {
            try
            {
                imageModel.RemovePicture(thumbPath);
                return "success";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [HttpPost]
        public JObject GetHandlers()
        {
            try
            {
                JObject handler_list = new JObject();

                List<string> list = WebClient.Instance.handlers;
                JArray arr = new JArray();
                foreach (string item in list)
                {
                    arr.Add(item);
                }

                handler_list["Handlers"] = arr;
                return handler_list;
            }
            catch(Exception e)
            {
                JObject handler_list = new JObject();
                handler_list["Handlers"] = new JArray(e.ToString());
               
                return handler_list;
            }
        }
        [HttpPost]
        public string RemoveHandler(string path)
        {
            try
            {
                WebClient.Instance.RemoveHandler(path);
                return "Success";
            }
            catch(Exception)
            {
                return "Fail";
            }
        }
    }
}