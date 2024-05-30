using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MafiaOnline.RoleCards
{
    internal class Sheriff : Card
    {
        public override string? Asset { get; set; }
        public override int RoleNumber { get; set; }
        public Sheriff()
        {
            Asset = "sheriff.png";
            RoleNumber = 4;
        }

        public override List<Player> RoleAction(List<Player> players, int playerId)
        {
            
            return players;
        }
    }
}
