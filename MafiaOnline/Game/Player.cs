﻿using MafiaOnline.RoleCards;
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
        public Card? card; 
        public string Name { get; set; }
        public int votesNumber;
        public int id;
        public bool IsAlive { get; set; }
        public Player() 
        { 
            IsAlive = true;
        }
    }
}
