using ImageService.Commands;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Command
{
    class FinishImageTransferCommand : ICommand
    {
        ImageServer serv;

        public FinishImageTransferCommand(ImageServer srv)
        {
            serv = srv;
        }
        public string Execute(string[] args, out bool result)
        {
            if (!serv.pathList.Any())
            {
                result = false;
                return "";
            }
            else
            {
                // turn base64 string to image
                string encodedImage = args[0];
                byte[] byteImage = Convert.FromBase64String(encodedImage);
                Image image = (Bitmap)((new ImageConverter()).ConvertFrom(byteImage));
                // save image in one of the handled paths
                image.Save(Path.Combine(serv.pathList[0], args[1]));
                result = true;
                return "";
            }
        }
    }
}
