using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaOnline.Network;

namespace MafiaOnline.Services
{
    internal class HostService: IHostService
    {
        private Host _host;
        public HostService() 
        {
            _host = new Host(9850);
        }

        public Host GetHost()
        {
            return _host;
        }
    }
}
