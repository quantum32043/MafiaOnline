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

        private async void CreateHost(object? sender, EventArgs e)
        {
            //_hostService = Handler!.MauiContext!.Services.GetService<IHostService>()!;
            //_host = _hostService!.GetHost();

            await Navigation.PushModalAsync(new HostPage());
        }

        //private void OnCreateHost(object sender, EventArgs e)
        //{
        //    Host host = new Host(9850);
        //    host.Start();

        //}

        private void OnConnectToHost(object sender, EventArgs e) 
        {
            Player player = new Player("Alexander");
            Client client = new Client();
            client.Join(player);
        }
    }

}
