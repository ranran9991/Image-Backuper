using System;
using Newtonsoft.Json.Linq;
namespace ImageService.Model
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

            JArray args = JArray.FromObject(Args);
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
            JArray args = (JArray)cmJson["Args"];

            string[] Args = args.ToObject<string[]>();
           

            // parse path
            string path = (string)cmJson["RequestDirPath"];

            CommandRecievedEventArgs cmArgs = new CommandRecievedEventArgs(id, Args, path);
            return cmArgs;
        }


    }
}
