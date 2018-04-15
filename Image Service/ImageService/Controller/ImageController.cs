using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_Model;                      // The Model Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModel Model)
        {
            m_Model = Model;                    // Storing the Model Of The System
            commands = new Dictionary<int, ICommand>();
            
            commands.Add((int)CommandEnum.NewFileCommand, new NewFileCommand(m_Model));
            commands.Add((int)CommandEnum.CloseCommand, new CloseCommand());
        }
        /// <summary>
        /// Exectures a command using the Command map
        /// </summary>
        /// <param name="commandID"></param>
        /// <param name="args"></param>
        /// <param name="resultSuccesful"></param>
        /// <returns></returns>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            ICommand command = commands[commandID];
            Task<string> t = Task<string>.Run(() => command.Execute(args, out bool temp));
            string res = t.Result;
            // this is an akward solution to the problem that i cant bass resultSuccesful
            // to an ansynchronious method.
            if (res == null)
                //if the function returned null it wasn't sucessful
            {
                resultSuccesful = false;
            }
            else
            {
                resultSuccesful = true;

            }
            return res;
        }
    }
}
