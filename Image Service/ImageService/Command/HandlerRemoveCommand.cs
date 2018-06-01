using ImageService.Commands;
using Infrastructure;
using ImageService.Model;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Command
{
    class HandlerRemoveCommand : ICommand
    {
        public ImageServer server;

        public HandlerRemoveCommand(ImageServer srv)
        {
            server = srv;
        }
        public string Execute(string[] args, out bool result)
        {

            string path = args[0];
            server.closeSpecificHandler(path);
            CommandRecievedEventArgs cmndRecieved = new CommandRecievedEventArgs((int)CommandEnum.HandlerRemoveCommand, args, null);
            server.RaiseNotifiyEvent(cmndRecieved);
            result = true;
            return cmndRecieved.ToJSON();
        }
    }
}
