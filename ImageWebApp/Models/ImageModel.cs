using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageWebApp.Models
{
    public class ImageModel
    {
        List<Thumbnail> thumbnails;

        public ImageModel()
        {
            thumbnails = new List<Thumbnail>
            {
                new Thumbnail("img0", "\\images\\image.jpg",2, 2012),
                new Thumbnail("img1" ,"\\images\\image1.jpg", 3, 1996),
                new Thumbnail("img2", "\\images\\image2.jpeg", 5, 2001),
                new Thumbnail("img3", "\\images\\image3.jpg", 12, 1832),
                new Thumbnail("img3", "\\images\\image4.jpg", 9, 1771)
            };
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
        /// <summary>
        /// given the thumbnail path returns the real image relative path
        /// </summary>
        /// <param name="thumbPath"></param>
        /// <returns></returns>
        public string GetRealImagePath(string thumbPath)
        {
            return "\\images\\image2.jpeg";
        }
        /// <summary>
        /// Removes the picture and the thumbnail given the thumbnail path
        /// </summary>
        /// <param name="path"></param>
        public void RemovePicture(string path)
        {   
            foreach(Thumbnail thumb in thumbnails)
            {
                thumbnails.Clear();
            }
        }

    }
}