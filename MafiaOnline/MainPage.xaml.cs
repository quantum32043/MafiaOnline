using System.Net;
using System.Net.Sockets;
using MafiaOnline.Network;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;
using MafiaOnline.Services;

namespace MafiaOnline
{
    public partial class MainPage : ContentPage
    {
        private IHostService _hostService;
        private Host _host;

        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLogin(object? sender, EventArgs e)
        {
            Button pressedButton = (Button)sender!;
            await Navigation.PushModalAsync(new LoginPage(pressedButton.Text));
        }
    }

}
