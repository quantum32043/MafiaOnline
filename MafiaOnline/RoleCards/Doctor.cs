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
            //if (Application.Current.RequestedTheme == AppTheme.Dark)
            //{
            //    asset = "dark_doctor.png";
            //}
            //else if (Application.Current.RequestedTheme == AppTheme.Light)
            //{
            //    asset = "doctor.png";
            //}
            asset = "doctor.png";
            RoleNumber = 3;
        }

        public void Heal() { }
    }
}
