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
            config = client.GetConfig();
            client.CommandReceived += RemoveHandler;
            client.Register();
        }

        public ConfigData config { get; set; }

        public void RemoveHandler(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.HandlerRemoveCommand)
            {
                config.Handler.Remove(e.RequestDirPath);
            }
        }

        public void DeleteHandler(string dir)
        {
            client.SendCommand(new CommandRecievedEventArgs((int)CommandEnum.HandlerRemoveCommand, null, dir));
        }
    }
}
