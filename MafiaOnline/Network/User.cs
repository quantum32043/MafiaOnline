using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MafiaOnline.Network
{
    internal abstract class User
    {
        private IPAddress _ip;
        private int _port;
        public IPAddress IP { get { return _ip; } }
        public int Port { get { return _port; } }
        protected User(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
        }
        public abstract void Create();
    }
}
