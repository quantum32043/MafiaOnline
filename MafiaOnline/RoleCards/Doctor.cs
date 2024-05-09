using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.RoleCards
{
    internal class Doctor : Card
    {
        public Doctor() 
        {
            asset = "doctor.png";
        }

        public void Heal() { }
    }
}
