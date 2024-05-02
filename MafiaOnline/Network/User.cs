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
        public IPAddress IP { get { return _ip; } protected set { _ip = value; } }
        public int Port { get { return _port; } protected set { _port = value; } }
        //public abstract void Create();
    }
}
