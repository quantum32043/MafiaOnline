using MafiaOnline.Network;
using MafiaOnline.Services;

namespace MafiaOnline;

public partial class HostPage : ContentPage
{
    private IHostService _hostService;
    private Host _host;
    private IClientService _clientService;
    private Client _client;
    private Player _player;

    public HostPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await Task.Delay(1000);

        _hostService = Handler!.MauiContext!.Services.GetService<IHostService>()!;
        _host = _hostService!.GetHost();
        _host.Start();
        _clientService = Handler!.MauiContext!.Services.GetService<IClientService>()!;
        _client = _clientService!.GetClient();
        _player = _clientService!.GetPlayer();
        await _client.Join(_player);
        DisplayPlayers();
    }
    //protected override async OnDisappearing()
    //{
    //    _host.ClientsWaiting = false;
    //}

    private async void OnStart(object? sender, EventArgs e)
    {
        _host.ClientsWaiting = false;
        _host.SendStartPackage();
        await Navigation.PushModalAsync(new GamePage());
    }

    private async void DisplayPlayers()
    {
        while (_host.ClientsWaiting)
        {
            ConnectionsLabel.Text = "";
            List<Player> players = [.._host.connections!.Keys.ToList()];
            foreach (Player player in players)
            {
                ConnectionsLabel.Text += player.Name + "\n";
                Console.WriteLine(player.Name);
            }
            await Task.Delay(500);
        }
    }
}