using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
       
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingModal m_logging;
        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();           // The Watcher of the Dir
        private string m_path;                              // The Path of directory
    

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

		public DirectoyHandler(IImageController controller)
        {
            m_controller = controller;
        }

        public void StartHandleDirectory(string dirPath)
        {
            m_path = dirPath;
            string[] filters = { "*.jpg", "*.png", "*.gif", "*.bmp" };
            foreach (string filter in filters)
            {
                FileSystemWatcher watcher = new FileSystemWatcher(m_path, filter);
                watcher.Created += new FileSystemEventHandler(OnCreated);
                watchers.Add(watcher);
                watcher.EnableRaisingEvents = true;
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                CloseHandler();
                return;
            }

            if (e.RequestDirPath == m_path)
            {
                m_logging.Log(DateTime.Now.ToString() + " Command Accepted To Directory " + e.RequestDirPath, MessageTypeEnum.INFO);
                bool result;
                m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string[] args = { e.FullPath };
            bool result;
            m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out result);
        }

        private void CloseHandler()
        {
            foreach (FileSystemWatcher watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
                m_logging.Log("Handler Stopped Watching In Directory " + m_path, MessageTypeEnum.INFO);
            }
        }
    }
}
