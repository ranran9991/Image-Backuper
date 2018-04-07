using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class CloseCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return null;
        }
    }
}
