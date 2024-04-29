using MafiaOnline.RoleCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline
{
    internal class Player
    {
        Card? _card; 
        string _name;
        int _port;
        int votesNumber;
        int id;
        public bool IsAlive { get; set; }
        public Card? Card { get; set; }
        public Player(string name) 
        { 
            _name = name;
            IsAlive = true;
        }
    }
}
