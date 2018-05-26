using Image_Service.ImageService.Server;
using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        public delegate void NotifyClients(CommandRecievedEventArgs cmndRecieved);
        public static event NotifyClients NotifyHandlerRemoved;
        private IImageController m_controller;
        private ILoggingModel m_logger;
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        public List<string> pathList;

        public ImageServer(string[] paths, IImageServiceModel imageModel, ILoggingModel log, IImageController controller)
        {
            m_logger = log;
            m_controller = controller;
            pathList = new List<string>(paths);
            foreach (string path in paths)
            {
                CreateHandler(path);
            }
        }
        public void RaiseNotifiyEvent(CommandRecievedEventArgs args)
        {
            NotifyHandlerRemoved?.Invoke(args);
        }
        /// <summary>
        /// Creates handler for a given directory path
        /// </summary>
        /// <param name="directory"></param>
        private void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logger);
            handler.StartHandleDirectory(directory);
            handler.DirectoryClose += onDirectoryError;
            m_logger.Log(DateTime.Now.ToString() + " deployed handler in directory " + directory, MessageTypeEnum.INFO);
            CommandRecieved += handler.OnCommandRecieved;
        }
        /// <summary>
        /// sends a command to the handlers that registered in the CommandRecieved event
        /// </summary>
        /// <param name="command"></param>
        private void SendCommand(CommandRecievedEventArgs command)
        {
            CommandRecieved?.Invoke(this, command);
        }
        /// <summary>
        /// Closes the server and invoking the closure of the handlers through the 
        /// appropriate event
        /// </summary>
        public void CloseServer()
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, new string[] { }, "");
            CommandRecieved?.Invoke(this, command);
        }
        /// <summary>
        /// handlers an error in a directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDirectoryError(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler handler = (IDirectoryHandler) sender;
            CommandRecieved -= handler.OnCommandRecieved;
            pathList.Remove(e.DirectoryPath);
            CommandRecievedEventArgs cmdRecieved = new CommandRecievedEventArgs((int)CommandEnum.HandlerRemoveCommand, null, e.DirectoryPath);
            NotifyHandlerRemoved?.Invoke(cmdRecieved);
        }
        /// <summary>
        /// Gets a path for a handler, closes that handler
        /// </summary>
        /// <param name="path"></param>
        public void closeSpecificHandler(string path)
        {
            pathList.Remove(path);
            CommandRecievedEventArgs cmdRecieved = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, path);
            CommandRecieved?.Invoke(this, cmdRecieved);
            m_logger.Log(DateTime.Now.ToString() + " Closing handler at path " + path, MessageTypeEnum.INFO);

        }

    }
}
