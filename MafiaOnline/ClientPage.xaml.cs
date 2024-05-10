using MafiaOnline.Network;
using MafiaOnline.Services;

namespace MafiaOnline;

public partial class ClientPage : ContentPage
{
    private IClientService _clientService;
    private Client _client;
    private Player _player;
    private bool clientConnected;

    public ClientPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        clientConnected = true;

        await Task.Delay(500);

        _clientService = Handler!.MauiContext!.Services.GetService<IClientService>()!;
        _client = _clientService!.GetClient();
        _player = _clientService!.GetPlayer();
        await _client.Join(_player);
        DisplayPlayers();
    }

    //protected async override void OnDisappearing()
    //{
    //    base.OnDisappearing();
    //    _client.Disconnect();
    //    clientConnected = false;
    //    await Navigation.PopToRootAsync();
    //}

    private async void DisplayPlayers()
    {
        List<Player> players = new List<Player>();
        while (_client.waitingServer && clientConnected)
        {
            players = _client.ReceivePreStartInfo();
            ConnectionsLabel.Text = "";
            if (_client.waitingServer)
            {
                foreach (Player player in players)
                {
                    ConnectionsLabel.Text += player.Name + "\n";
                    Console.WriteLine(player.Name);
                }

                await Task.Delay(500);
            }
        }
        if (clientConnected)
        {
            Console.WriteLine("Start game!");
            _player.id = _client.id;
            await Navigation.PushModalAsync(new GamePage(false));
        }
    }
}