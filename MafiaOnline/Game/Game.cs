using MafiaOnline.RoleCards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MafiaOnline
{
    internal class Game
    {
        [JsonProperty]
        public List<Player> players { get; set; } = new List<Player>();
        [JsonProperty]
        public List<Card> _cards { get; set; } = new List<Card>();
        [JsonProperty]
        public GameDayTime _currentDayTime { get; set; } = GameDayTime.Day;
        [JsonProperty]
        public GamePhase _currentGamePhase { get; set; }
        [JsonProperty]
        public bool GameOver { get; set; }
        [JsonProperty]
        public int _dayNumber { get; set; } = 1;
        [JsonProperty]
        public int _mafiaCount { get; set; }
        [JsonProperty]
        public int _peacefulCount { get; set; }

        public event EventHandler? PhaseChanged;
        public event EventHandler? RoleActionRequired;
        public event EventHandler? UpdatePlayers;

        private TaskCompletionSource<bool>? _actionCompletionSource;

        public Game()
        {
            GameOver = false;
        }

        public void Initialize()
        {
            if (players == null) throw new ArgumentNullException(nameof(players));

            _mafiaCount = (int)Math.Round(players.Count / 3.5);
            _peacefulCount = players.Count - _mafiaCount;
            int citizenCount = players.Count - _mafiaCount - 2;
            _currentGamePhase = GamePhase.GeneralDiscussionPhase;

            for (int i = 0; i < _mafiaCount; i++)
            {
                _cards.Add(new Mafia());
            }

            for (int i = 0; i < citizenCount; i++)
            {
                _cards.Add(new Citizen());
            }

            _cards.Add(new Doctor());
            _cards.Add(new Sheriff());

            AssignRoles();

        }

        public void Start()
        {
            RunGameLoop();
        }

        public void AssignRoles()
        {
            Random random = new Random();
            _cards = _cards.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < players.Count; i++)
            {
                players[i].card = _cards[i];
            }
        }

        private async void RunGameLoop()
        {
            while (!IsGameOver())
            {
                switch (_currentDayTime)
                {
                    case GameDayTime.Day:
                        await RunDayPhase();
                        break;
                    case GameDayTime.Night:
                        await RunNightPhase();
                        break;
                }
            }
        }

        private async Task RunDayPhase()
        {
            await GeneralDiscussion();
            await IndividualDiscussion();
            if (_dayNumber != 1)
            {
                await Voting();
            }
            TransitionToPhase(GameDayTime.Night);
        }

        private async Task RunNightPhase()
        {
            await MafiaRoleplaying();
            await SheriffRoleplaying();
            await DoctorRoleplaying();
            OnUpdatePlayers();
            TransitionToPhase(GameDayTime.Day);
        }

        private async Task GeneralDiscussion()
        {
            Console.WriteLine("Фаза общей дискуссии");
            _actionCompletionSource = new TaskCompletionSource<bool>();
            _currentGamePhase = GamePhase.GeneralDiscussionPhase;
            OnPhaseChanged();
            await _actionCompletionSource.Task;
        }

        private async Task IndividualDiscussion()
        {
            Console.WriteLine("Фаза индивидуальной дискуссии");
            _actionCompletionSource = new TaskCompletionSource<bool>();
            _currentGamePhase = GamePhase.IndividualDiscussionPhase;
            OnPhaseChanged();
            await _actionCompletionSource.Task;
        }

        private async Task Voting()
        {
            Console.WriteLine("Фаза голосования");
            _actionCompletionSource = new TaskCompletionSource<bool>();
            _currentGamePhase = GamePhase.VotingPhase;
            OnPhaseChanged();
            await _actionCompletionSource.Task;
        }

        private async Task MafiaRoleplaying()
        {
            Console.WriteLine("Фаза мафии");
            _actionCompletionSource = new TaskCompletionSource<bool>();
            _currentGamePhase = GamePhase.MafiaPhase;
            OnPhaseChanged();
            //OnRoleActionRequired();
            await _actionCompletionSource.Task; // Ожидание завершения действия мафии
        }

        private async Task SheriffRoleplaying()
        {
            Console.WriteLine("Фаза шерифа");
            _actionCompletionSource = new TaskCompletionSource<bool>();
            _currentGamePhase = GamePhase.SheriffPhase;
            OnPhaseChanged();
            //OnRoleActionRequired();
            await _actionCompletionSource.Task; // Ожидание завершения действия шерифа
        }

        private async Task DoctorRoleplaying()
        {
            Console.WriteLine("Фаза доктора");
            _actionCompletionSource = new TaskCompletionSource<bool>();
            _currentGamePhase = GamePhase.DoctorPhase;
            OnPhaseChanged();
            //OnRoleActionRequired();
            await _actionCompletionSource.Task; // Ожидание завершения действия доктора
        }

        public void CompleteAction()
        {
            _actionCompletionSource?.TrySetResult(true);
        }

        private void TransitionToPhase(GameDayTime nextPhase)
        {
            _currentDayTime = nextPhase;
        }

        private bool IsGameOver()
        {
            if (_mafiaCount > _peacefulCount)
            {
                GameOver = true;
            }
            else
            {
                GameOver = false;
            }
            return GameOver;
        }

        protected virtual void OnPhaseChanged()
        {
            PhaseChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRoleActionRequired()
        {
            RoleActionRequired?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpdatePlayers()
        {
            UpdatePlayers?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum GameDayTime
    {
        Day,
        Night
    }

    public enum GamePhase
    {
        MafiaPhase,
        SheriffPhase,
        DoctorPhase,
        IndividualDiscussionPhase,
        GeneralDiscussionPhase,
        VotingPhase
    }

    public enum GameState
    {
        Initial,
        InProgress,
        Completed
    }
}