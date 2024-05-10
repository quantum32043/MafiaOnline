using MafiaOnline.Network;
using MafiaOnline.Services;

namespace MafiaOnline;

public partial class GamePage : ContentPage
{
    private readonly bool _isHost;
    private IGameService _gameService;
    private Game _game;
    private IHostService _hostService;
    private Host _host;
    private IClientService _clientService;
    private Client _client;
    private Player _player;
    private Label _selectedLabel;
    public GamePage(bool isHost)
	{
        _isHost = isHost;
		InitializeComponent();

        bool isZoomed = false;

        RoleCard.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                if (!isZoomed)
                {
                    Console.WriteLine(ViewLayout.Width);
                    await RoleCard.ScaleTo(2.6, 200);
                    ViewLayout.WidthRequest = ViewLayout.Width * 2.6;
                    ViewLayout.HeightRequest = ViewLayout.Height * 2.6;
                    Console.WriteLine(ViewLayout.WidthRequest);
                    isZoomed = true;
                }
                else
                {
                    ViewLayout.WidthRequest = ViewLayout.Width / 2.6;
                    ViewLayout.HeightRequest = ViewLayout.Height / 2.6;
                    await RoleCard.ScaleTo(1, 200);
                    isZoomed = false;
                }
            })
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(1000);

        _gameService = Handler!.MauiContext!.Services.GetService<IGameService>();
        _game = _gameService.GetGame();
        _clientService = Handler!.MauiContext!.Services.GetService<IClientService>()!;
        _client = _clientService.GetClient();

        if (_isHost ) 
        {  
            _hostService = Handler!.MauiContext!.Services.GetService<IHostService>()!;
            _host = _hostService.GetHost();
            _game.players = _host.connections.Keys.ToList();
            _game.Start();
            _game.AssignRoles();
            _host.SendGameInfo(_game);

            //Инициализация игроков в игре через список у хоста
            //Запуск игры
            //Раздача ролей
            //Отправка всех данных о игре клиентам

        }
        else
        {
            //Инициализация игроков в игре через список у клиента
            //Запуск игры
            //Прием данных о игре от сервера
            _game = _client.ReceiveGameInfo();
            _game.Start();
        }
        _player = _clientService.GetPlayer();
        foreach (var player in _game.players)
        {
            if (_player.id == player.id)
            {
                _player = player;
            }
        };
        RoleCard.Source = _player.card.asset;
        Console.WriteLine(_player);
        DisplayPlayers();
    }

    private void DisplayPlayers()
    {
        foreach (var player in _game.players) 
        {
            if (player != _player)
            {
                var frame = new Frame
                {
                    HeightRequest = 40,
                    Margin = new Thickness(0, 5, 0, 0),
                    Padding = new Thickness(0),
                    CornerRadius = 10,
                    Content = new Label
                    {
                        Text = player.Name,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 22,
                        HeightRequest = 40,
                        AutomationId = player.id.ToString()
                    }
                };

                // Добавление TapGestureRecognizer к Label
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnLabelTapped;
                ((Label)frame.Content).GestureRecognizers.Add(tapGestureRecognizer);

                // Добавление Frame на страницу
                PlayerList.Add(frame);
            }
        }
    }

    private void OnLabelTapped(object sender, EventArgs e)
    {
        // Снимаем выделение с предыдущего Label
        if (_selectedLabel != null)
        {
            _selectedLabel.BackgroundColor = Colors.LightGray;
        }

        // Выделяем новый Label
        _selectedLabel = sender as Label;
        _selectedLabel.BackgroundColor = Colors.Red;
    }

    // Обработчик нажатия кнопки для получения информации
    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (_selectedLabel != null)
        {
            // Получаем информацию из выделенного Label
            Console.WriteLine(_selectedLabel.AutomationId + " : " + _selectedLabel.Text);
            // Теперь вы можете использовать playerName для дальнейших действий
        }
    }

    private async void OnRoleAction(object sender, EventArgs e)
    {
        Console.WriteLine("Action!!");
    }
}