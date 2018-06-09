using Infrastructure;
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

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size

        public ImageServiceModel(string outPutDir, int thumbnailSize)
        {
            m_OutputFolder = outPutDir;
            m_thumbnailSize = thumbnailSize;
        }

        #endregion
        /// <summary>
        /// This function gets a path to a file
        /// copies it to the correct place in the output folder
        /// creates a thumbnail and puts it in the correct folder as well
        /// </summary> 
        /// <param name="imPath"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        string IImageServiceModel.AddFile(string imPath, out bool result)
        {
            // lets the thread sleep so the image can fully download to the directory
            Thread.Sleep(200);
            
            if (!Directory.Exists(m_OutputFolder))
            {
                string error = CreateDir(m_OutputFolder, out result);
                if (!result)
                {
                    return null;
                }
            }
            if (!File.Exists(imPath))
            {
                result = false;
                return null;
            }
            //handeling thumbnail
           
            Image image = Image.FromFile(imPath);
            Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize,
                                                 () => true, IntPtr.Zero);
            string thumbnailDir = Path.Combine(m_OutputFolder, "Thumbnails");
            if (!Directory.Exists(thumbnailDir))
            {
                CreateDir(thumbnailDir, out result);
                if (!result)
                {
                    return null;
                }
            }
            // saves thumbnail to the path
            // loop is used to handle cases in which an image with
            // the same name already exists
            string tempPath = imPath;
            string addedStr = "";
            int i = 1;
            while (true)
            {
                tempPath = imPath;
                if(!File.Exists(Path.Combine(thumbnailDir, Path.GetFileName(tempPath))))
                {
                    thumb.Save(Path.Combine(thumbnailDir, Path.GetFileName(tempPath)));
                    break;
                }
                else
                {
                    addedStr = i.ToString();
                    tempPath = AddSuffix(tempPath, i.ToString());
                    i++;
                }
            }
            image.Dispose();
            thumb.Dispose();
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
                    return null;
                }
            }
            //create month dir
            string monthPath = Path.Combine(yearPath, (createTime.Month).ToString());
            if (!Directory.Exists(monthPath))
            {
                CreateDir(monthPath, out result);
                if (!result)
                {
                    return null;
                }
            }
            string outPath = Path.Combine(monthPath, Path.GetFileName(imPath));
            // move file to correct dir, loop used to handle
            // cases where image with same name already exists in the directory
            addedStr = "";
            tempPath = outPath;
            i = 1;
            while (true){
                tempPath = outPath;
                if (!File.Exists(tempPath))
                {
                    MoveFile(imPath, tempPath, out result);
                    break;
                }
                else { 
                    addedStr = i.ToString();
                    tempPath = AddSuffix(tempPath, i.ToString());
                    i++;
                }
            }
           
            return outPath;
        }

        string CreateDir(string path, out bool result)
        {
            try
            {
                Directory.CreateDirectory(path);
                result = true;
                return null;
            }
            catch(Exception e)
            { 
                result = false;
                return e.ToString();
            }
        }
        /// <summary>
        /// Moves a file from inputPath to outputPath
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <param name="result"></param>
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
        /// <summary>
        /// Gets the date in which a picture in path was taken
        /// if a date doesnt exist, take the creation date
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// <summary>
        /// adds the suffix to the file name while ignoring the extension
        /// for example if filename = C:\image.jpg and suffix = 1 it will output
        /// C:image1.jpg
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        string AddSuffix(string filename, string suffix)
        {
            string fDir = Path.GetDirectoryName(filename);
            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = Path.GetExtension(filename);
            return Path.Combine(fDir, String.Concat(fName, suffix, fExt));
        }

    }

}
