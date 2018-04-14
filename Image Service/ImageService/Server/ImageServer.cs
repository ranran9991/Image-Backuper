using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        private IImageController m_controller;
        private ILoggingModal m_logger;
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        public ImageServer(string[] paths, IImageServiceModal imageModal, ILoggingModal log, IImageController controller)
        {
            m_logger = log;
            m_controller = controller;
            foreach (string path in paths)
            {
                CreateHandler(path);
            }
        }

        private void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logger);
            handler.StartHandleDirectory(directory);
            handler.DirectoryClose += DirectoryError;
            m_logger.Log(DateTime.Now.ToString() + " deployed handler in directory " + directory, MessageTypeEnum.INFO);
            CommandRecieved += handler.OnCommandRecieved;
        }

        private void SendCommand(CommandRecievedEventArgs command)
        {
            CommandRecieved?.Invoke(this, command);
        }

        public void CloseServer()
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, new string[] { }, "");
            CommandRecieved?.Invoke(this, command);
        }

        private void DirectoryError(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler handler = (IDirectoryHandler) sender;
            CommandRecieved -= handler.OnCommandRecieved;
        }
    }
}
