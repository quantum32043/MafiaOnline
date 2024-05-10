using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.RoleCards
{
    internal class Mafia : Card
    {
        public Mafia()
        {
            //if (Application.Current.RequestedTheme == AppTheme.Dark)
            //{
            //    asset = "dark_mafia.png";
            //}
            //else if (Application.Current.RequestedTheme == AppTheme.Light)
            //{
            //    asset = "mafia.png";
            //}
            asset = "mafia.png";
        }

        public void Shot() { }
    }
}
