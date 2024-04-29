using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.RoleCards
{
    internal abstract class Card
    {
        string? _role;
        string? _asset;
        public string? Role { get => _role; protected set { _role = value; } }
        public string? Asset { get => _asset; protected set { _asset = value; } }
    }
}
