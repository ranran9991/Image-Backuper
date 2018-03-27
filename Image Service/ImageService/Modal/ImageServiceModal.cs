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

        #endregion
        string IImageServiceModal.AddFile(string path, out bool result)
        {
            // This function gets a path to a file
            // copies it to the correct place in the output folder
            // creates a thumbnail and puts it in the correct folder as well
            if (!Directory.Exists(m_OutputFolder))
            {
                CreateDir(path, out result);
                if (!result)
                {
                    return "FileCreationError";
                }
            }
            if (!File.Exists(path))
            {
                result = false;
                return "ImageError";
            }
            //create year dir
            DateTime createTime = File.GetCreationTime(path);
            string yearPath = Path.Combine(m_OutputFolder, (createTime.Year).ToString());
            if (!Directory.Exists(yearPath))
            {
                CreateDir(yearPath, out bool createSuccessful);
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
            //move file to correct dir
            MoveFile(path, monthPath, out result);
            if (!result)
            {
                return "FIleMoveError";
            }
            //TODO: ADD THUMBNAIL CREATION
            return null;
        }

        void CreateDir(string path, out bool result)
        {
            try
            {
                Directory.CreateDirectory(path);
                result = true;
            }
            catch(Exception e)
            {
                result = false;
            }
        }

        void MoveFile(string inputPath, string outputPath, out bool result)
        {
            try
            {
                File.Copy(inputPath, outputPath);
                result = true;
            }
            catch(Exception e)
            {
                result = false;
            }
        }

    }
}
