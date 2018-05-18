using ImageService.Controller;
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
        public IClientHandler ch;

        public TcpServer(string ip, int port, IClientHandler handler)
        {
            Ip = ip;
            Port = port;
            ch = handler;
            m_mutex = new Mutex();
        }

        public void Notify(CommandRecievedEventArgs cmdRecieved)
        {
            foreach(TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                string jsonCommand = cmdRecieved.ToJSON();
                m_mutex.WaitOne();
                writer.Write(jsonCommand);
                m_mutex.ReleaseMutex();
            }
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Ip), Port);
            listener = new TcpListener(ep);
            listener.Start();
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        
                        m_mutex.WaitOne();
                        clients.Add(client);
                        m_muted.ReleaseMutex();
                        
                        ch.HandleClient(client, clients);
                    }
                    catch (SocketException)
                    {
                        clients.Remove(client);
                        break;
                    }
                }
                
            });
            task.Start();
        }
        
        public void Stop()
        {
            listener.Stop();
        }
    }

       
}
