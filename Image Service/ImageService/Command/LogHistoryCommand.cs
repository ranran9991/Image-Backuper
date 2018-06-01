using ImageService.Commands;
using Infrastructure;
using ImageService.Logging.Model;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Command
{
    class LogHistoryCommand : ICommand
    {
        public ILoggingModel m_log;
        public LogHistoryCommand(ILoggingModel mod)
        {
            m_log = mod;
        }
        public string Execute(string[] args, out bool result)
        {
            string logHistory = m_log.LogToJSON();
            string[] cmndArgs = { logHistory };
            CommandRecievedEventArgs cmndRecieved = new CommandRecievedEventArgs((int)CommandEnum.LogHistoryCommand, cmndArgs, null);
            result = true;
            return cmndRecieved.ToJSON();
        }

    }
}
