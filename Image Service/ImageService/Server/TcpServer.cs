using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Server
{
    class TcpServer
    {
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
        }

        public void Notify(string msg)
        {
            foreach(TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                writer.Write(msg);
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
                        Console.WriteLine("Got new connection");
                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }
        
        public void Stop()
        {
            listener.Stop();
        }
    }

       
}
