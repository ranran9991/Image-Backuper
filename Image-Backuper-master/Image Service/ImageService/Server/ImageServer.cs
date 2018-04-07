using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        private IImageController controller;
        private ILoggingModal logger;
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        public ImageServer(string[] paths, IImageServiceModal imageModal)
        {
            controller = new ImageController(imageModal);
            foreach (string path in paths)
            {
                CreateHandler(path);
            }
        }

        private void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoyHandler(controller);
            handler.StartHandleDirectory(directory);
            CommandRecieved += handler.OnCommandRecieved;
        }

        private void SendCommand(CommandRecievedEventArgs command)
        {
            CommandRecieved?.Invoke(this, command);
        }

        public void CloseServer()
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, new string[] { }, "");
        }
    }
}
