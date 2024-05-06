using MafiaOnline.Network;
using MafiaOnline.Services;


namespace MafiaOnline;

public partial class LoginPage : ContentPage
{
    private IClientService _clientService;
    private Player _player;
    public LoginPage(string buttonPressed)
	{
		InitializeComponent();
		if (buttonPressed == "Create")
		{
			NextButton.Text = "Create";
			NextButton.Clicked += OnCreateHost;
		}
		else if (buttonPressed == "Connect")
		{
			NextButton.Text = "Join";	
			NextButton.Clicked += OnConnectToHost;
		}

		LoginEntry.TextChanged += (s, e) =>
		{
			NextButton.IsEnabled = !string.IsNullOrEmpty(LoginEntry.Text);
		};
    }

    protected override async void OnAppearing()
    {
        await Task.Delay(1000);

        _clientService = Handler!.MauiContext!.Services.GetService<IClientService>()!;
        _player = _clientService!.GetPlayer();
    }

    private async void OnBack(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}

	private async void OnCreateHost(object? sender, EventArgs e)
	{
		_player.Name = LoginEntry.Text;
		await Navigation.PushModalAsync(new HostPage());
	}

	private async void OnConnectToHost(object sender, EventArgs e)
	{
        _player.Name = LoginEntry.Text;
        await Navigation.PushModalAsync(new ClientPage());
	}
}