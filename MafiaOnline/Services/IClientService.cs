using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaOnline.Network;

namespace MafiaOnline.Services
{
    internal interface IClientService
    {
        Client GetClient();
        Player GetPlayer();
    }
}
