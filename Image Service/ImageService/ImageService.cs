using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Configuration;
using ImageService.Logging.Model;
using ImageService.Server;
using ImageService.Model;
using ImageService.Controller;
using System.IO;
using Image_Service.ImageService.Server;

public enum ServiceState
{
    SERVICE_STOPPED = 0x00000001,
    SERVICE_START_PENDING = 0x00000002,
    SERVICE_STOP_PENDING = 0x00000003,
    SERVICE_RUNNING = 0x00000004,
    SERVICE_CONTINUE_PENDING = 0x00000005,
    SERVICE_PAUSE_PENDING = 0x00000006,
    SERVICE_PAUSED = 0x00000007,
}

[StructLayout(LayoutKind.Sequential)]
public struct ServiceStatus
{
    public int dwServiceType;
    public ServiceState dwCurrentState;
    public int dwControlsAccepted;
    public int dwWin32ExitCode;
    public int dwServiceSpecificExitCode;
    public int dwCheckPoint;
    public int dwWaitHint;
};

namespace ImageService
{
    public partial class ImageService : ServiceBase
    {
        private ImageServer m_imageServer;          // The Image Server
        private IImageServiceModel imageModel;
        private IImageController controller;
        private ILoggingModel logging;
        private TcpServer srv;
        public ImageService()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            // Initialize the service
            InitializeComponent();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStart(string[] args)
        {
            string[] paths = ConfigurationManager.AppSettings["Handler"].Split(';'); 
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string sourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            int thumbnailSize = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);


            List<string> pathsList = new List<string>();

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    pathsList.Add(path);
                }
                else
                {
                    eventLogger.WriteEntry(DateTime.Now.ToString() + " " + path + ": Invalid Path To Directory");
                }
            }

            paths = pathsList.ToArray<string>();

            eventLogger = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(sourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    sourceName, logName);
            }
            eventLogger.Source = sourceName;
            eventLogger.Log = logName;

            eventLogger.WriteEntry(DateTime.Now.ToString() + " Service Started");


            // initialize logging
            logging = new LoggingModel();
            logging.MessageRecieved += OnMessageRecieved;
            // initialize servers controllers and modals
            imageModel = new ImageServiceModel(outputDir, thumbnailSize);
            controller = new ImageController(imageModel, logging);
            m_imageServer = new ImageServer(paths, imageModel, logging, controller);
            srv = new TcpServer("127.0.0.1", 8000, (IClientHandler)controller);
            ImageServer.NotifyHandlerRemoved += srv.Notify;
            this.logging.NotifyLogChanged += srv.Notify;

            // start handeling clients
            srv.Start();
        }

        protected override void OnStop()
        {
            eventLogger.WriteEntry(DateTime.Now.ToString() + " Service Stopped");
            m_imageServer?.CloseServer();
            srv.Stop();
        }
        public void OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.Status)
            {
                case MessageTypeEnum.INFO:
                    eventLogger.WriteEntry(e.Message, EventLogEntryType.Information);
                    break;
                case MessageTypeEnum.FAIL:
                    eventLogger.WriteEntry(e.Message, EventLogEntryType.FailureAudit);
                    break;
                case MessageTypeEnum.WARNING:
                    eventLogger.WriteEntry(e.Message, EventLogEntryType.Warning);
                    break;
            }
        }
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
