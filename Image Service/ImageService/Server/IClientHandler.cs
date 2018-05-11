using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Image_Service.ImageService.Server
{
    interface IClientHandler
    {
        void HandleClient(TcpClient client);
    }
}
