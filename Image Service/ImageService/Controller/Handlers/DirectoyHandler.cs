using ImageService.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
       
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingModel m_logging;
        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();           // The Watcher of the Dir
        private string m_path;                              // The Path of directory
    

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

		public DirectoyHandler(IImageController controller, ILoggingModel log)
        {
            m_controller = controller;
            m_logging = log;

        }
        /// <summary>
        /// this is called as an initialization when starting to handle
        /// a directory
        /// </summary>
        /// <param name="dirPath"></param>
        public void StartHandleDirectory(string dirPath)
        {
            m_path = dirPath;
            m_logging.Log(DateTime.Now.ToString() + " started handeling directory at path" + m_path, MessageTypeEnum.INFO);
            string[] filters = { "*.jpg", "*.png", "*.gif", "*.bmp" };
            foreach (string filter in filters)
            {
                try
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(m_path, filter);
                    watcher.Created += new FileSystemEventHandler(OnCreated);
                    watcher.Error += new ErrorEventHandler(OnFileWatcherError);
                    watchers.Add(watcher);
                    watcher.EnableRaisingEvents = true;
                }
                catch (Exception e)
                {
                    m_logging.Log(e.ToString(), MessageTypeEnum.FAIL);
                }
            }
        }
        /// <summary>
        /// Passes a command to the controller or closes the handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.CloseCommand && 
                (e.RequestDirPath == m_path || e.RequestDirPath == null))
            {
                CloseHandler();
                return;
            }

            if (e.RequestDirPath == m_path)
            {
                m_logging.Log(DateTime.Now.ToString() + " Command Accepted To Directory " + e.RequestDirPath, MessageTypeEnum.INFO);
                bool result;
                string res = m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
                if(result == false)
                {
                    m_logging.Log(DateTime.Now.ToString() + res, MessageTypeEnum.FAIL);
                    // handeling controller error
                    DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path, res));
                    CloseHandler();

                }
            }
        }
        /// <summary>
        /// This function is called when an image is created
        /// it reports to the log and executes the command
        /// in the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string[] args = { e.FullPath };
            bool result;
            string res;
            res = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out result);
            // if command was unsuccessful
            if (!result)
            {
                m_logging.Log(DateTime.Now.ToString() + " " + res, MessageTypeEnum.FAIL);
                DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path, res));
                CloseHandler();
                return;
            }
            m_logging.Log(DateTime.Now.ToString() + " Image Created: " + res, MessageTypeEnum.INFO);

        }
        /// <summary>
        /// This function is invoked when filewatcher has an error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileWatcherError(object sender, ErrorEventArgs e)
        {
            m_logging.Log(DateTime.Now.ToString() + " Error Detected In Handler Of Directory " + m_path, MessageTypeEnum.FAIL);
            DirectoryCloseEventArgs args = new DirectoryCloseEventArgs(m_path, e.GetException().ToString());
            DirectoryClose?.Invoke(this, args);
            CloseHandler();
        }
        /// <summary>
        /// Closes the handler and reports to the logger
        /// </summary>
        private void CloseHandler()
        {
            foreach (FileSystemWatcher watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
            }
        }
    }
}
