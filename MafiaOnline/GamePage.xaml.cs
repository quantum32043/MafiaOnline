using MafiaOnline.Network;
using MafiaOnline.Services;

namespace MafiaOnline;

public partial class GamePage : ContentPage
{
    private readonly bool _isHost;
    private readonly IStartGameStrategy _gameStrategy;
    private IGameService _gameService;
    private Game _game;
    private IHostService _hostService;
    private Host _host;
    private IClientService _clientService;
    private Client _client;
    private Player _player;
    public GamePage(IStartGameStrategy gameStrategy, bool isHost)
	{
        _gameStrategy = gameStrategy;
        _isHost = isHost;
		InitializeComponent();
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
            _client.ReceiveGameInfo(ref _game);
            _game.Start();
        }
        _player = _clientService.GetPlayer();
        foreach (var player in _game.players)
        {
            if (player.id == _player.id)
            {
                _player = player;
            }
        };
        RoleCard.Source = _player.card.asset;
    }
}