using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageWebApp.Models
{
    public class ImageModel
    {
        List<Thumbnail> thumbnails;

        public ImageModel()
        {
            /*thumbnails = new List<Thumbnail>
            {
                new Thumbnail("img0", "\\images\\image.jpg",2, 2012),
                new Thumbnail("img1" ,"\\images\\image1.jpg", 3, 1996),
                new Thumbnail("img2", "\\images\\image2.jpeg", 5, 2001),
                new Thumbnail("img3", "\\images\\image3.jpg", 12, 1832),
                new Thumbnail("img3", "\\images\\image4.jpg", 9, 1771)
            }; */
            // i left it here for your use

            thumbnails = GetThumbnails();
        }
        public string ThumbnailsToJSON()
        {
            JObject thumbList = new JObject();
            JArray arr = new JArray();

            foreach (Thumbnail thumb in thumbnails)
            {
                arr.Add(thumb.ToJSON());
            }

            thumbList["Thumbnails"] = arr;
            return thumbList.ToString();
        }

        public List<Thumbnail> GetThumbnails()
        {
            List<Thumbnail> thumbs = new List<Thumbnail>();
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string thumbDirPath = outputDir + "\\Thumbnails";
            Console.WriteLine(thumbDirPath);

            string[] years = Directory.GetDirectories(outputDir);

            foreach (string year in years)
            {
                if (year != thumbDirPath)
                {
                    string[] months = Directory.GetDirectories(year);

                    foreach (string month in months)
                    {
                        string[] images = Directory.GetFiles(month);

                        foreach (string image in images)
                        {
                            string name = image.Substring(month.Length + 1);
                            string path = thumbDirPath + "\\" + name;
                            int _month = int.Parse(month.Substring(year.Length + 1));
                            int _year = int.Parse(year.Substring(outputDir.Length + 1));

                            Thumbnail thumbnail = new Thumbnail(name, path, _month, _year);
                            thumbs.Add(thumbnail);
                        }
                    }
                }
            }
            return thumbs;
        }

        public string GetRealImagePath(string thumbPath)
        {
            foreach (Thumbnail thumb in thumbnails)
            {
                if (thumb.Path == thumbPath)
                {
                    return ConfigurationManager.AppSettings["OutputDir"] + "\\" + thumb.Year +
                        "\\" + thumb.Month + "\\" + thumb.Name;
                }
            }
            return "";
        }

        public void RemovePicture(string path)
        {
            string realImage = GetRealImagePath(path);
            if (realImage == "") return;
            File.Delete(path);
            File.Delete(realImage);

            foreach (Thumbnail thumb in thumbnails)
            {
                if (thumb.Path == path)
                {
                    thumbnails.Remove(thumb);
                    break;
                }
            }
        }
    }
}