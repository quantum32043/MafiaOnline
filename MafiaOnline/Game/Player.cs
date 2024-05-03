using MafiaOnline.RoleCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MafiaOnline
{
    internal class Player
    {
        //Card? card; 
        public string Name { get; set; }
        public int votesNumber;
        public int id;
        public bool IsAlive { get; set; }
        //public Card? Card { get; set; }
        public Player(string name) 
        { 
            Name = name;
            IsAlive = true;
        }
    }
}
