using MafiaOnline.Network;
using MafiaOnline.Services;

namespace MafiaOnline;

public partial class ClientPage : ContentPage
{
    private IClientService _clientService;
    private Client _client;
    private Player _player;

    public ClientPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await Task.Delay(1000);

        _clientService = Handler!.MauiContext!.Services.GetService<IClientService>()!;
        _client = _clientService!.GetClient();
        _player = _clientService!.GetPlayer();
        await _client.Join(_player);
        //ViewPlayers();
    }



    //private async void ViewPlayers()
    //{
    //    while (_host.ClientsWaiting)
    //    {
    //        ConnectionsLabel.Text = "";
    //        List<Player> players = _host.connections!.Keys.ToList();
    //        foreach (Player player in players)
    //        {
    //            ConnectionsLabel.Text += player.Name + "\n";
    //            Console.WriteLine(player.Name);
    //        }
    //        await Task.Delay(5000);
    //    }
    //}
}