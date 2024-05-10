using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.RoleCards
{
    internal class Sheriff : Card
    {
        public Sheriff()
        {
            //if (Application.Current.RequestedTheme == AppTheme.Dark)
            //{
            //    asset = "dark_sheriff.png";
            //}
            //else if (Application.Current.RequestedTheme == AppTheme.Light)
            //{
            //    asset = "sheriff.png";
            //}
            asset = "sheriff.png";
        }

        public void Check() { }
    }
}
