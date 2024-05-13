using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.RoleCards
{
    internal class Citizen : Card
    {
        public Citizen()
        {
            //if (Application.Current.RequestedTheme == AppTheme.Dark)
            //{
            //    asset = "dark_citizen.png";
            //}
            //else if (Application.Current.RequestedTheme == AppTheme.Light)
            //{
            //    asset = "citizen.png";
            //}
            asset = "citizen.png";
            RoleNumber = -1;
        }
    }
}
