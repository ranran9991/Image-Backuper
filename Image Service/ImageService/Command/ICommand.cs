﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public interface ICommand
    {
        // The Function That will Execute The Command
        string Execute(string[] args, out bool result); 
    }
}
