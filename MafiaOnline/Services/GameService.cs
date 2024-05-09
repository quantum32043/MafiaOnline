using MafiaOnline.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.Services
{
    internal class GameService : IGameService
    {
        private Game _game;
        public GameService()
        {
            _game = new Game();
        }

        public Game GetGame()
        {
            return _game;
        }
    }
}
