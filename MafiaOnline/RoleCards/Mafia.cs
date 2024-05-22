using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MafiaOnline.RoleCards
{
    internal class Mafia : Card
    {
        public override string? Asset { get; set; }
        public override int RoleNumber { get; set; }
        public Mafia()
        {
            Asset = "mafia.png";
            RoleNumber = 1;
        }

        public override List<Player> RoleAction(List<Player> players, int playerId)
        {
            foreach (Player player in players)
            {
                if (player.id == playerId)
                {
                    player.IsAlive = false;
                    Console.WriteLine(player.ToString() + " was killed");
                    break;
                }
            }
            return players;
        }
    }
}
