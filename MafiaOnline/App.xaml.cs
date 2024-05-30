using Microsoft.Maui.ApplicationModel;

namespace MafiaOnline
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Проверьте, включена ли темная тема
            //if (Application.Current.UserAppTheme == AppTheme.Dark)
            //{
            //    Application.Current.Resources = new DarkTheme();
            //}
            //else
            //{
            //    Application.Current.Resources = new LightTheme();
            //}
            //Application.Current.Resources = new DarkTheme();
            Application.Current.Resources = new LightTheme();
        }
    }

}
