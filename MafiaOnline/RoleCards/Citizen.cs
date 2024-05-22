using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MafiaOnline.RoleCards
{
    internal class Citizen : Card
    {
        public override string? Asset {  get; set; }
        public override int RoleNumber { get; set; }
        public Citizen()
        {
            Asset = "citizen.png";
            RoleNumber = 2;
        }

        public override List<Player> RoleAction(List<Player> players, int playerId)
        {
            return players;
        }
    }
}
