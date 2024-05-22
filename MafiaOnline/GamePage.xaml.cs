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
        }
        else
        {
            _client.ReceiveGameInfo(ref _game);
        }
        // Подписка на события
        _game.PhaseChanged += OnPhaseChanged;
        _game.RoleActionRequired += OnRoleActionRequired;
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
        }
    }

    private async void OnRoleAction(object sender, EventArgs e)
    {
        Console.WriteLine("Action!!");
        // Реализация действия роли
        if (_player.card is Mafia)
        {
            // Действие для мафии
        }
        else if (_player.card is Doctor)
        {
            // Действие для доктора
        }
        else if (_player.card is Sheriff)
        {
            // Действие для шерифа
        }

        // Уведомляем игру о завершении действия роли
        _game.CompleteAction();
    }

    private async void OnPhaseChanged(object sender, EventArgs e)
    {
        Console.WriteLine(_game._currentGamePhase.ToString());
        switch (_game._currentGamePhase)
        {
            case GamePhase.GeneralDiscussionPhase:
                PhaseLabel.Text = "Общее время";
                await RunGeneralDiscussion();
                break;
            case GamePhase.IndividualDiscussionPhase:
                PhaseLabel.Text = "Индивидуальная дискуссия";
                await RunIndividualDiscussion();
                break;
            case GamePhase.VotingPhase:
                PhaseLabel.Text = "Голосование";
                await RunVoting();
                break;
            case GamePhase.MafiaPhase:
                PhaseLabel.Text = "Ход мафии";
                await RunMafiaRoleplaying();
                break;
            case GamePhase.SheriffPhase:
                PhaseLabel.Text = "Ход шерифа";
                await RunSheriffRoleplaying();
                break;
            case GamePhase.DoctorPhase:
                PhaseLabel.Text = "Ход доктора";
                await RunDoctorRoleplaying();
                break;
        }
    }

    private void OnRoleActionRequired(object sender, EventArgs e) //работает некорректно
    {
        // Здесь вы можете вызвать действия в зависимости от роли игрока
        if (_player.card is Mafia)
        {
            // Показать интерфейс для действия мафии
            Console.WriteLine("Мафия должна выполнить действие.");
        }
        else if (_player.card is Doctor)
        {
            // Показать интерфейс для действия доктора
            Console.WriteLine("Доктор должен выполнить действие.");
        }
        else if (_player.card is Sheriff)
        {
            // Показать интерфейс для действия шерифа
            Console.WriteLine("Шериф должен выполнить действие.");
        }
    }

    private async Task RunGeneralDiscussion()
    {
        StartTimer(5);
    }

    private async Task RunIndividualDiscussion()
    {
        StartTimer(_game.players.Count * 5);
        //int i = 0;
        //while (_discussionTimeRemaining > 0) 
        //{
        //    if (_discussionTimeRemaining % 5 == 0)
        //    {
        //        PhaseLabel.Text = "Индивидуальная дискуссия\n" + _game.players[i++].Name;
        //    }
        //}
    }

    private async Task RunVoting()
    {
        StartTimer(5);
    }

    private async Task RunMafiaRoleplaying()
    {
        // Реализация действий мафии
        Console.WriteLine("Мафия выполняет действие.");
        if (_player.card is Mafia) 
        {
            RoleButton.IsEnabled = true;
        }
        // Здесь вы можете показать интерфейс для действия мафии
    }

    private async Task RunSheriffRoleplaying()
    {
        // Реализация действий шерифа
        Console.WriteLine("Шериф выполняет действие.");
        if (_player.card is Sheriff)
        {
            RoleButton.IsEnabled = true;
        }
        // Здесь вы можете показать интерфейс для действия шерифа
    }

    private async Task RunDoctorRoleplaying()
    {
        // Реализация действий доктора
        Console.WriteLine("Доктор выполняет действие.");
        if (_player.card is Mafia)
        {
            RoleButton.IsEnabled = true;
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
            _timerLabel.Text = $"00:{_discussionTimeRemaining:00}";
            _discussionTimeRemaining--;
            return true; // Продолжаем таймер
        });
    }
}