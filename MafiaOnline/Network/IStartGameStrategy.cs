using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.Network
{
   public interface IStartGameStrategy
    {
        void StartGame();
        void ReceiveGameInfo();
        void SendGameInfo();
    }
}
