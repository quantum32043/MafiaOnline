using MafiaOnline.RoleCards;

namespace MafiaOnline.Game
{
    internal class Game
    {
        private List<Player> _players;
        private List<Card> _cards;
        private GamePhase _currentPhase;
        private int _dayNumber;
        private int _mafiaCount;
        private int _peacefulCount;

        public Game(List<Player> players)
        {
            _players = players;
            _cards = new List<Card>();
            _currentPhase = GamePhase.Day; // Игра начинается с дневной фазы
            _dayNumber = 1;
            _mafiaCount = (int)Math.Round(_players.Count / 3.5);
            _peacefulCount = _players.Count - _mafiaCount;
            int citizenCount = _players.Count - _mafiaCount - 2;

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

        private void AssignRoles()
        {
            Random random = new Random();
            _cards = _cards.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].card = _cards[i];
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
            foreach (var player in _players) 
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

