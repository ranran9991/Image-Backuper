using ImageService.Infrastructure;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel m_Model;

        public NewFileCommand(IImageServiceModel Model)
        {
            m_Model = Model;            // Storing the Model
        }

        public string Execute(string[] args, out bool result)
        {
            return  m_Model.AddFile(args[0], out result);
        }
    }
}
