using MafiaOnline.RoleCards;
using Newtonsoft.Json;

namespace MafiaOnline
{
    internal class Player: ICloneable
    {
        public Card? card { get; set; }

        public string Name { get; set; } = string.Empty;

        public int votesNumber { get; set; }

        public int id { get; set; }

        public bool IsAlive { get; set; }

        public Player()
        {
            IsAlive = true;
        }

        public object Clone()
        {
            return new Player() 
            {
                card = this.card,
                Name = this.Name,
                votesNumber = this.votesNumber,
                id = this.id,
                IsAlive = this.IsAlive
            };
        }
    }
}