using ImageService.Logging.Model;
using ImageService.Server;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Server
{
    class ImageReceiver
    {
        public int Port { get; set; }
        private TcpListener listener;
        private ImageServer serv;
        ILoggingModel logger;

        public ImageReceiver(int port, ImageServer server, ILoggingModel log)
        {
            serv = server;
            Port = port;
            logger = log;
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
                        logger.Log("Image broadcaster connected", MessageTypeEnum.INFO);
                        NetworkStream stream = client.GetStream();
                        BinaryReader reader = new BinaryReader(stream);
                        while (true)
                        {
                            // Handle client
                            byte[] rcvLenBytes = new byte[4];
                            reader.Read(rcvLenBytes, 0, 4);
                            int rcvLen = System.BitConverter.ToInt32(rcvLenBytes, 0);
                            byte[] rcvBytes = new byte[rcvLen];
                            reader.Read(rcvBytes, 0, rcvLen);
                            String rcv = System.Text.Encoding.ASCII.GetString(rcvBytes);

                            CommandRecievedEventArgs cmdArgs = CommandRecievedEventArgs.FromJSON(rcv);
                            if (cmdArgs.CommandID == (int)CommandEnum.StartImageTransfer)
                            {
                                string name = cmdArgs.Args[0];

                                // read image and save it
                                byte[] inputLenBytes = new byte[4];
                                reader.Read(inputLenBytes, 0, 4);
                                int inputLen = System.BitConverter.ToInt32(inputLenBytes, 0);
                                // loop to recieve all packets 
                                // the amount of bytes is too big to fit in one packet
                                // so the image spreads out across multiple ones
                                byte[] inputBytes = new byte[inputLen];
                                int i = 0;
                                do
                                {
                                    byte[] rbytes = new byte[inputLen - i];
                                    int numRead = reader.Read(rbytes, 0, inputLen - i);
                                    for(int j = i; j <  i + numRead; j++)
                                    {
                                        inputBytes[j] = rbytes[j - i];
                                    }
                                    i += numRead;

                                }
                                while (i != inputLen);
                                // save image
                                System.IO.File.WriteAllBytes(Path.Combine(serv.pathList[0], name), inputBytes);
                                
                                reader.Read(inputLenBytes, 0, 4);
                                inputLen = System.BitConverter.ToInt32(inputLenBytes, 0);
                                inputBytes = new byte[inputLen];
                                reader.Read(inputBytes, 0, inputLen);
                                String input = System.Text.Encoding.ASCII.GetString(inputBytes);
                                // file transfer finished
                                CommandRecievedEventArgs finishTransfer = CommandRecievedEventArgs.FromJSON(input.ToString());
                                if (finishTransfer.CommandID == (int)CommandEnum.FinishImageTransfer)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        logger.Log(DateTime.Now.ToString() + " Socket Exception ", MessageTypeEnum.FAIL);
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception in Receiver" + e.ToString(), MessageTypeEnum.FAIL);
                    }
                }
                   
            });
            task.Start();
        }
        private Image byteArrayToImage(byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);
            return img;
        }
        private void SaveImage(Image img, String name)
        {
            img.Save(Path.Combine(serv.pathList[0], name));
            return;
        }
    }
}
