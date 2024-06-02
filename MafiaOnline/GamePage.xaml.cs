using MafiaOnline.Network;
using MafiaOnline.RoleCards;
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
    private System.Timers.Timer _discussionTimer;
    private int _discussionTimeRemaining;
    private Label _timerLabel;
    private Dictionary<GamePhase, string> _gamePhaseMap { get; set; }

    public GamePage(bool isHost)
    {
        _isHost = isHost;
        InitializeComponent();

        bool isZoomed = false;

        _gamePhaseMap = new Dictionary<GamePhase, string>()
        {
            { GamePhase.MafiaPhase, "Ход мафии"},
            { GamePhase.SheriffPhase, "Ход шерифа"},
            { GamePhase.DoctorPhase, "Ход доктора"},
            { GamePhase.IndividualDiscussionPhase, "Индивидуальная дискуссия"},
            { GamePhase.GeneralDiscussionPhase, "Общее время"},
            { GamePhase.VotingPhase, "Голосование"}
        };

        RoleCard.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                if (!isZoomed)
                {
                    await RoleCard.ScaleTo(2.6, 200);
                    ViewLayout.WidthRequest = ViewLayout.Width * 2.6;
                    ViewLayout.HeightRequest = ViewLayout.Height * 2.6;
                    isZoomed = true;
                }
                else
                {
                    ViewLayout.WidthRequest = ViewLayout.Width;
                    ViewLayout.HeightRequest = ViewLayout.Height;
                    await RoleCard.ScaleTo(1, 200);
                    isZoomed = false;
                }
            })
        });

        _timerLabel = new Label
        {
            Text = "00:00",
            Margin = new Thickness(160, 10, 20, 0),
            FontSize = 24
        };
        PageView.Children.Add(_timerLabel);
        _timerLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(1000);

        _gameService = Handler!.MauiContext!.Services.GetService<IGameService>();
        _game = _gameService.GetGame();
        _clientService = Handler!.MauiContext!.Services.GetService<IClientService>()!;
        _client = _clientService.GetClient();

        if (_isHost)
        {
            _hostService = Handler!.MauiContext!.Services.GetService<IHostService>()!;
            _host = _hostService.GetHost();
            _game.players = _host.connections.Keys.ToList();
            _game.Initialize();
            _host.SendGameInfo(_game);
            _host.ForwardUpdateGamePackages();
            await Task.Delay(550);
        }
        // Подпишитесь на событие получения игры
        _client.GameReceived += UpdateGame;
        _client.ActionCompleted += CompleteAction;
        // Запускаем ReceiveGameInfo в отдельном потоке
        _ = Task.Run(async () => await _client.ReceiveGameInfo());
        await Task.Delay(500);
        // Подписка на события
        _game.PhaseChanged += OnPhaseChanged;
        _game.UpdatePlayers += UpdatePlayers;
        //_game.RoleActionRequired += OnRoleActionRequired;
        _game.Start();

        _player = _clientService.GetPlayer();
        foreach (var player in _game.players)
        {
            if (_player.id == player.id)
            {
                _player = player;
                break;
            }
        }
        foreach (var player in _game.players)
        {
            Console.WriteLine(player.Name + " : " + player.card.Asset);
        }
        RoleCard.Source = _player.card.Asset;
        Console.WriteLine(_player);
        DisplayPlayers();
    }

    private void UpdateGame(Game newGame)
    {
        _game.UpdateState(newGame);
        // Обновляем пользовательский интерфейс на главном потоке
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase];
        });

        // Здесь обновите пользовательский интерфейс или выполните другие действия
        Console.WriteLine("Получена новая игра");
    }


    private void DisplayPlayers()
    {
        foreach (var player in _game.players)
        {
            var content = new Label
            {
                Text = player.Name,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 22,
                HeightRequest = 40,
                AutomationId = player.id.ToString()
            };
            content.SetDynamicResource(Label.TextColorProperty, "TextColor");
            if (player != _player)
            {
                var frame = new Frame
                {
                    HeightRequest = 40,
                    Margin = new Thickness(0, 5, 0, 0),
                    Padding = new Thickness(0),
                    CornerRadius = 10,
                    Content = content
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

    private void CompleteAction(object sender, EventArgs e)
    {
        Console.WriteLine("Завершение хода");
        _game.CompleteAction();
    }

    private void UpdatePlayers(object sender, EventArgs e)
    {
        PlayerList.Clear();
        foreach (var player in _game.players)
        {
            if (player != _player)
            {
                var content = new Label
                {
                    Text = player.Name,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 22,
                    HeightRequest = 40,
                    AutomationId = player.id.ToString()
                };
                content.SetDynamicResource(Label.TextColorProperty, "TextColor");
                var frame = new Frame
                {
                    HeightRequest = 40,
                    Margin = new Thickness(0, 5, 0, 0),
                    Padding = new Thickness(0),
                    CornerRadius = 10,
                    Content = content
                };

                if (player.IsAlive)
                {
                    // Добавление TapGestureRecognizer к Label
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += OnLabelTapped;
                    ((Label)frame.Content).GestureRecognizers.Add(tapGestureRecognizer);
                }
                else
                {
                    frame.Content.BackgroundColor = Colors.DarkGray;
                    frame.Content.IsEnabled = false;
                    ((Label)frame.Content).Text += " (Dead)";
                }

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
        if (_selectedLabel != null)
        {
            _selectedLabel.BackgroundColor = Colors.Red;
        }
    }

    // Обработчик нажатия кнопки для получения информации
    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (_selectedLabel != null)
        {
            _game.players = _player.card.RoleAction(_game.players, Int32.Parse(_selectedLabel.AutomationId));
            RoleButton.IsEnabled = false;
            _client.SendUpdatedGameInfo(_game);
        }
    }

    private void Test(object sender, EventArgs e)
    {
        Console.WriteLine(_game._currentGamePhase.ToString());
        OnPhaseChanged(sender, e);

        //Console.WriteLine(_game._currentGamePhase.ToString());
    }

    private async void OnPhaseChanged(object sender, EventArgs e)
    {
        Console.WriteLine(_game._currentGamePhase.ToString());
        switch (_game._currentGamePhase)
        {
            case GamePhase.GeneralDiscussionPhase:
                MainThread.BeginInvokeOnMainThread(() => { PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase]; });
                await RunGeneralDiscussion();
                break;
            case GamePhase.IndividualDiscussionPhase:
                MainThread.BeginInvokeOnMainThread(() => { PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase]; });
                await RunIndividualDiscussion();
                break;
            case GamePhase.VotingPhase:
                MainThread.BeginInvokeOnMainThread(() => { PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase]; });
                await RunVoting();
                break;
            case GamePhase.MafiaPhase:
                MainThread.BeginInvokeOnMainThread(() => { PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase]; });
                await RunMafiaRoleplaying();
                break;
            case GamePhase.SheriffPhase:
                MainThread.BeginInvokeOnMainThread(() => { PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase]; });
                await RunSheriffRoleplaying();
                break;
            case GamePhase.DoctorPhase:
                MainThread.BeginInvokeOnMainThread(() => { PhaseLabel.Text = _gamePhaseMap[_game._currentGamePhase]; });
                await RunDoctorRoleplaying();
                break;
        }
    }


    private async Task RunGeneralDiscussion()
    {
        StartTimer(5);
    }

    private async Task RunIndividualDiscussion()
    {
        StartTimer(_game.players.Count * 5);
    }

    private async Task RunVoting()
    {
        StartTimer(5);
    }

    private async Task RunMafiaRoleplaying()
    {
        // Реализация действий мафии
        Console.WriteLine("Мафия выполняет действие.");
        if (_game.isRoleAlive(1))
        {
            if (_player.card is Mafia)
            {
                RoleButton.IsEnabled = true;
            }
        }
        else
        {
            _game.CompleteAction();
        }
    }

    private async Task RunSheriffRoleplaying()
    {
        // Реализация действий шерифа
        Console.WriteLine("Шериф выполняет действие.");
        if (_game.isRoleAlive(4))
        {
            if (_player.card is Sheriff)
            {
                RoleButton.IsEnabled = true;
            }
        }
        else
        {
            _game.CompleteAction();
        }
    }

    private async Task RunDoctorRoleplaying()
    {
        // Реализация действий доктора
        if (_game.isRoleAlive(3))
        {
            if (_player.card is Doctor)
            {
                RoleButton.IsEnabled = true;
            }
        }
        else
        {
            _game.CompleteAction();
        }
    }

    private async void StartTimer(int time)
    {
        _discussionTimeRemaining = time;
        this.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            if (_discussionTimeRemaining < 0)
            {
                _game.CompleteAction();
                return false; // Останавливаем таймер
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _timerLabel.Text = $"00:{_discussionTimeRemaining:00}";
            });
            _discussionTimeRemaining--;
            return true; // Продолжаем таймер
        });
    }
}