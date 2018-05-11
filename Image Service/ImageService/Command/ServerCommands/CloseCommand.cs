using ImageService.Infrastructure;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class CloseCommand : ICommand
    {
        public string path;
        public string Execute(string[] args, out bool result)
        {
            if(args.Length > 1)
            {
                path = args[1];
            }
            result = true;
            return null;
        }
    }
}
