using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaOnline.Network;

namespace MafiaOnline.Services
{
    internal class ClientService: IClientService
    {
        private Client _client;
        private Player _player;
        public ClientService() 
        {
            _client = new Client();
            _player = new Player();
        }

        public Client GetClient()
        {
            return _client;
        }

        public Player GetPlayer() 
        {
            return (Player)_player;
        }
    }
}
