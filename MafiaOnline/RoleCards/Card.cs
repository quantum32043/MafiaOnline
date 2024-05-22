using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonSubTypes;
using Newtonsoft.Json;

namespace MafiaOnline.RoleCards
{
    internal abstract class Card
    {
        public abstract string? Asset { get; set; }
        public abstract int RoleNumber { get; set; }
        public abstract List<Player> RoleAction(List<Player> players, int playerId);
    }
}
