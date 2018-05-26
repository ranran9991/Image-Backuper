using Image_Service.ImageService.Command;
using Image_Service.ImageService.Server;
using ImageService.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Model;
using ImageService.Model;
using ImageService.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IClientHandler, IImageController
    {
        public ImageServer server;
        public ImageServer Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
                commands.Add((int)CommandEnum.HandlerRemoveCommand, new HandlerRemoveCommand(server));
                commands.Add((int)CommandEnum.ConfigCommand, new ConfigCommand(server));
            }
        }
        private IImageServiceModel m_Model;               // The Model Object
        public Dictionary<int, ICommand> commands;
        public ILoggingModel m_log;
        public ImageController(IImageServiceModel Model, ILoggingModel log)
        {
            m_Model = Model;             // Storing the Model Of The System
            m_log = log;
            commands = new Dictionary<int, ICommand>();
            
            commands.Add((int)CommandEnum.NewFileCommand, new NewFileCommand(m_Model));
            commands.Add((int)CommandEnum.LogHistoryCommand, new LogHistoryCommand(m_log));
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
            // this is an akward solution to the problem that i cant pass resultSuccesful
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

        public void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            Task task = new Task(() => {
                bool success;
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true);
                BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true);
                while (true)
                {
                    try
                    {
                        string commandLine = reader.ReadString();
                        
                        CommandRecievedEventArgs cmdArgs = CommandRecievedEventArgs.FromJSON(commandLine.ToString());
                        if (cmdArgs.CommandID == (int)CommandEnum.CloseClientCommand)
                        {
                            // if the client wants to disconnect
                            clients.Remove(client);
                            client.Close();
                        }
                        // execute the command
                        string cmdOut = ExecuteCommand(cmdArgs.CommandID, cmdArgs.Args, out success);

                        // write output of command to client
                        writer.Write(cmdOut);
                    }
                    catch (JsonException)
                    {
                        m_log.Log(DateTime.Now.ToString() + " Caught JSON Exception in client", MessageTypeEnum.WARNING);
                        writer.Close();
                        reader.Close();
                        return;
                    }
                    catch (Exception)
                    {
                        m_log.Log(DateTime.Now.ToString() + " I/O Exception Caught in client", MessageTypeEnum.WARNING);
                        writer.Close();
                        reader.Close();
                        return;
                    }
              
                }
            });
            task.Start();
        }
    }
}
