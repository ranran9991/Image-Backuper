using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Server
{
    public class Client
    {
        TcpClient client;
        NetworkStream stream;
        StreamReader reader;
        StreamWriter writer;

        public Client(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }


        public void send(string msg)
        {
            writer.Write(msg);
        }

        public string recieve()
        {
            // Buffer to store the response bytes.
            char[] readBuffer = new char[client.ReceiveBufferSize];
            reader.Read(readBuffer, 0, client.ReceiveBufferSize);
            return readBuffer.ToString();
        }
    }
}
