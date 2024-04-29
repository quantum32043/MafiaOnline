using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MafiaOnline.Network
{
    internal class Host : User
    {
        private TcpListener? listener;
        private Dictionary<Player, TcpClient> connections;
        public Host(IPAddress ip, int port) : base(ip, port){ }
        public override void Create()
        {
            listener = new TcpListener(this.IP, this.Port);
            listener.Start();
        }
        public async void Connect(Player player)
        {
            while (true)
            {
                var client = await listener!.AcceptTcpClientAsync();
                connections.Add(player, client);
            }
        }
    }
}
