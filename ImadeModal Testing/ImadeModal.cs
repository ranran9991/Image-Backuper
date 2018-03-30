using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Design;
using System.Drawing;
namespace ImadeModal_Testing
{
    public class ImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        public ImageServiceModal(string path, int size)
        {
            this.m_OutputFolder = path;
            this.m_thumbnailSize = size;
        }
        #endregion
        public string AddFile(string imPath, out bool result)
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
            //create year dir
            DateTime createTime = File.GetCreationTime(imPath);
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
            string outPath = Path.Combine(monthPath, Path.GetFileName(imPath));
            //move file to correct dir
            MoveFile(imPath, outPath, out result);
            if (!result)
            {
                return "FileMoveError";
            }
            //creating the thumbnail
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
            thumb.Save(Path.Combine(thumbnailDir, Path.GetFileName(imPath)));
            return null;
        }

        void CreateDir(string path, out bool result)
        {
            try
            {
                Directory.CreateDirectory(path);
                result = true;
            }
            catch (Exception)
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
            catch (Exception)
            {
                result = false;
            }
        }

    }
}


