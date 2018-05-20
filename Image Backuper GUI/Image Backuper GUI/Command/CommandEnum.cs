using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Backuper_GUI.Command
{
    public enum CommandEnum : int
    {
        NewFileCommand,
        CloseCommand,
        HandlerRemoveCommand,
        ConfigCommand,
        LogCommand,
        CloseClientCommand
    }
}
