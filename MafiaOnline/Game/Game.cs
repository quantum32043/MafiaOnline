using MafiaOnline.RoleCards;
using Newtonsoft.Json;

namespace MafiaOnline
{
    internal class Game
    {
        public List<Player> players { get; set; }
        [JsonProperty]
        public List<Card> _cards { get; set; }
        [JsonProperty]
        public GamePhase _currentPhase { get;  set; }
        [JsonProperty]
        public int _dayNumber { get;  set; }
        [JsonProperty]
        public int _mafiaCount { get;  set; }
        [JsonProperty]
        public int _peacefulCount { get;  set; }

        public Game()
        {
            _cards = new List<Card>();
            _currentPhase = GamePhase.Day; // Игра начинается с дневной фазы
            _dayNumber = 1;
        }

        public void Start()
        {
            _mafiaCount = (int)Math.Round(players.Count / 3.5);
            _peacefulCount = players.Count - _mafiaCount;
            int citizenCount = players.Count - _mafiaCount - 2;

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
            while (!IsGameOver())
            {
                switch (_currentPhase)
                {
                    case GamePhase.Day:
                        DayPhase();
                        break;
                    case GamePhase.Night:
                        NightPhase();
                        break;
                }
            }
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

        private void DayPhase()
        {
            // Логика дневной фазы: обсуждение, голосование и т.д.
            // ...
            TransitionToPhase(GamePhase.Night);
        }

        private void NightPhase()
        {
            // Логика ночной фазы: отыгрышь ролей
            // ...
            TransitionToPhase(GamePhase.Day);
        }

        private void GeneralDiscussion()
        {

        }

        private void IndividualDiscusion()
        {
            foreach (var player in players) 
            {
            
            }
        }

        private void Voting()
        {
             
        }

        private void TransitionToPhase(GamePhase nextPhase)
        {
            // Переход между фазами
            _currentPhase = nextPhase;
        }

        private bool IsGameOver()
        {
            bool GameOver;
            if (_mafiaCount > _peacefulCount) GameOver = false;
            else GameOver = true;
            return GameOver;
        }
    }

    public enum GamePhase
    {
        Day,
        Night
    }
    public enum GameState
{
    Initial,
    InProgress,
    Completed
}
}

