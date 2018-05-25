using Image_Backuper_GUI.Client;
using Image_Backuper_GUI.Command;
using Image_Backuper_GUI.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Backuper_GUI.Model
{
    class SettingsModel
    {
        public GUIClient client { get; set; }

        public SettingsModel()
        {
            client = GUIClient.Instance;
            config = new ConfigData(new List<string>(), "", "", "", 0);
            client.CommandReceived += HandleCommand;
            client.Register();
            client.SendCommand(new CommandRecievedEventArgs((int)CommandEnum.ConfigCommand, null, ""));
        }

        public ConfigData config { get; set; }

        public void HandleCommand(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.HandlerRemoveCommand)
            {
                config.Handler.Remove(e.RequestDirPath);
                return;
            }

            if (e.CommandID == (int)CommandEnum.ConfigCommand)
            {
                config.Set(ConfigData.FromJSON(e.Args[0]));
            }
        }

        public void DeleteHandler(string dir)
        {
            client.SendCommand(new CommandRecievedEventArgs((int)CommandEnum.HandlerRemoveCommand, null, dir));
        }
    }
}
