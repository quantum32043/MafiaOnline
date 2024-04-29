using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MafiaOnline.Network
{
    internal class Client : User
    {
        TcpClient? tcpClient;
        public Client(IPAddress ip, int port) : base(ip, port) { }
        public override void Create()
        {
            try
            {
                tcpClient = new TcpClient(this.IP.ToString(), this.Port);
            }
            catch
            {
            
            }
        }
        async public void Join(IPAddress ip, int port)
        {
            try
            {
                await tcpClient!.ConnectAsync(ip, port);
            }
            catch
            { 
            
            }
        }
    }
}
