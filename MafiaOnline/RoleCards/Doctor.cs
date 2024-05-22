using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MafiaOnline.RoleCards
{
    internal class Doctor : Card
    {
        public override string? Asset { get; set; }
        public override int RoleNumber { get; set; }
        public Doctor() 
        {
            Asset = "doctor.png";
            RoleNumber = 3;
        }

        public override List<Player> RoleAction(List<Player> players, int playerId) 
        {
            foreach (Player player in players) 
            {
                if (player.id == playerId) 
                {
                    player.IsAlive = true;
                    Console.WriteLine(player.ToString() + " was healed");
                    break;
                }
            }
            return players;
        }
    }
}
