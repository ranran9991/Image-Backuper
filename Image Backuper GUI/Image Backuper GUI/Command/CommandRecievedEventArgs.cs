using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Backuper_GUI.Command
{
    public class CommandRecievedEventArgs : EventArgs
    {
        public int CommandID { get; set; }      // The Command ID
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; }  // The Request Directory

        public CommandRecievedEventArgs(int id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }

        public string ToJSON()
        {
            JObject eventobj = new JObject();
            eventobj["CommandID"] = CommandID;

            JArray args = new JArray(Args);
            eventobj["Args"] = args;

            eventobj["RequestDirPath"] = RequestDirPath;

            return eventobj.ToString();
        }

        public static CommandRecievedEventArgs FromJSON(string str)
        {
            JObject cmJson = JObject.Parse(str);
            // parse id
            int id = (int)cmJson["CommandID"];

            // parse args
            JArray JArgs = (JArray)cmJson["Args"];
            string[] args = new string[JArgs.Count];
            int i = 0;
            foreach (JToken tok in JArgs.Children())
            {
                args[i] = (string)tok;
                i++;
            }

            // parse path
            string path = (string)cmJson["RequestDirPath"];

            CommandRecievedEventArgs cmArgs = new CommandRecievedEventArgs(id, args, path);
            return cmArgs;
        }
    }
}
