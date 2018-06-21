using ImageService.Controller;
using Infrastructure;
using ImageService.Logging.Model;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Server
{
    class TcpServer
    {
        Mutex m_mutex;
        public string Ip { get; set; }
        public int Port { get; set; }
        public List<TcpClient> clients;
        private TcpListener listener;
        private ILoggingModel logger;
        public IClientHandler ch;

        public TcpServer(string ip, int port, IClientHandler handler, ILoggingModel log)
        {
            Ip = ip;
            Port = port;
            ch = handler;
            logger = log;
            clients = new List<TcpClient>();
            m_mutex = new Mutex();
        }

        public void Notify(CommandRecievedEventArgs cmdRecieved)
        {

            foreach(TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true);

                string jsonCommand = cmdRecieved.ToJSON();

                m_mutex.WaitOne();
                writer.Write(jsonCommand);
                m_mutex.ReleaseMutex();
            }
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, Port);
            listener = new TcpListener(ep);
            listener.Start();
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        logger.Log(DateTime.Now.ToString() + " client connected", MessageTypeEnum.INFO);

                        

                        ch.HandleClient(client, clients);

                        m_mutex.WaitOne();
                        clients.Add(client);
                        m_mutex.ReleaseMutex();
                    }
                    catch (SocketException)
                    {
                        logger.Log(DateTime.Now.ToString() + " Socket Exception ", MessageTypeEnum.FAIL);
                    }
                }
            });
            task.Start();
        }
        
        public void Stop()
        {
            // tell clients that server is closing
            CommandRecievedEventArgs closeCmnd = new CommandRecievedEventArgs((int)CommandEnum.CloseClientCommand, null, null);
            Notify(closeCmnd);
            // stop listening to new clients
            listener.Stop();
        }
    }

       
}
