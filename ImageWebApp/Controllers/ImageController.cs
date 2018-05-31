using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageWebApp.Controllers
{
    public class ImageController : Controller
    {
        // GET: Image
        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Config()
        {
            return View();
        }
        public ActionResult Log()
        {
            return View();
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
    }
}