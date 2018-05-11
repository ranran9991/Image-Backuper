using Image_Service.ImageService.Server;
using ImageService.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Command
{
    class LogCommand : ICommand
    {
        Client client;
        public LogRecorder logRec;
        
        public LogCommand(LogRecorder log)
        {
            logRec = log;
        }
        public string Execute(string[] args, out bool result)
        {
            try
            {
                result = true;
                return logRec.ToJSON();
            }
            catch (Exception)
            {
                result = false;
                return null;
            }
        }
    }
}
