using MafiaOnline.Network;
using MafiaOnline.Services;

namespace MafiaOnline;

public partial class HostPage : ContentPage
{
    private IHostService _hostService;
    private Host _host;

    public HostPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        //base.OnAppearing();

        await Task.Delay(1000); // Добавьте небольшую задержку

        _hostService = Handler!.MauiContext!.Services.GetService<IHostService>()!;
        _host = _hostService!.GetHost();
        ViewPlayers();
    }

    private async void ViewPlayers()
    {
        while (true)
        {
            List<Player> players = _host.connections!.Keys.ToList();
            foreach (Player player in players)
            {
                //ConnectionsLabel.Text += player.Name + "\n";
                Console.WriteLine(player.Name);
            }
            await Task.Delay(5000);
        }
    }
}