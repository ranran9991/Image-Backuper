using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size

        public ImageServiceModal(string outPutDir, int thumbnailSize)
        {
            m_OutputFolder = outPutDir;
            m_thumbnailSize = thumbnailSize;
        }

        #endregion
        string IImageServiceModal.AddFile(string imPath, out bool result)
        {
            // This function gets a path to a file
            // copies it to the correct place in the output folder
            // creates a thumbnail and puts it in the correct folder as well
            if (!Directory.Exists(m_OutputFolder))
            {
                CreateDir(imPath, out result);
                if (!result)
                {
                    return "FileCreationError";
                }
            }
            if (!File.Exists(imPath))
            {
                result = false;
                return "ImageError";
            }
            //handeling thumbnail
           
            Image image = Image.FromFile(imPath);
            Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize,
                                                 () => true, IntPtr.Zero);
            string thumbnailDir = Path.Combine(m_OutputFolder, "thumbnails");
            if (!Directory.Exists(thumbnailDir))
            {
                CreateDir(thumbnailDir, out result);
                if (!result)
                {
                    return "FileCreationError";
                }
            }
            // saves thumbnail to the path
            // loop is used to handle cases in which an image with
            // the same name already exists
            while (true)
            {
                string tempPath = imPath;
                string addedStr = "";
                int i = 0;
                try
                {
                    tempPath = imPath + addedStr;
                    thumb.Save(Path.Combine(thumbnailDir, Path.GetFileName(tempPath)));
                    break;
                }
                catch (Exception)
                {
                    addedStr = i.ToString();
                    i++;
                }
            }
            image.Dispose();
            //handeling backup
            DateTime createTime = getDateTaken(imPath);
            string yearPath = Path.Combine(m_OutputFolder, (createTime.Year).ToString());
            if (!Directory.Exists(yearPath))
            {
                bool createSuccessful;
                CreateDir(yearPath, out createSuccessful);
                if (!createSuccessful)
                {
                    result = false;
                    return "FileCreationError";
                }
            }
            //create month dir
            string monthPath = Path.Combine(yearPath, (createTime.Month).ToString());
            if (!Directory.Exists(monthPath))
            {
                CreateDir(monthPath, out result);
                if (!result)
                {
                    return "FileCreationError";
                }
            }
            string outPath = Path.Combine(monthPath, Path.GetFileName(imPath));
            // move file to correct dir, loop used to handle
            // cases where image with same name already exists in the directory
            while(true){
                string addedStr = "";
                string tempPath = outPath;
                int i = 0;
                MoveFile(imPath, tempPath, out result);
                if (!result)
                {
                    addedStr = i.ToString();
                    tempPath = outPath + addedStr;
                    i++;
                    continue;
                }
                break;
            }
           
            return null;
        }

        void CreateDir(string path, out bool result)
        {
            try
            {
                Directory.CreateDirectory(path);
                result = true;
            }
            catch(Exception)
            {
                result = false;
            }
        }

        void MoveFile(string inputPath, string outputPath, out bool result)
        {
            try
            {
                File.Move(inputPath, outputPath);
                result = true;
            }
            catch(Exception)
            {
                result = false;
            }
        }

        public static DateTime getDateTaken(string path)
        {
            try
            {
                Regex r = new Regex(":");
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch (Exception)
            {
                return File.GetCreationTime(path);
            }

        }

    }
}
