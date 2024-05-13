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
        public GameDayTime _currentDayTime { get; set; }
        [JsonProperty]
        public GamePhase _currentGamePhase { get; set; }
        [JsonProperty]
        public bool GameOver { get; set; }
        [JsonProperty]
        public int _dayNumber { get; set; }
        [JsonProperty]
        public int _mafiaCount { get; set; }
        [JsonProperty]
        public int _peacefulCount { get; set; }

        public Game()
        {
            _cards = new List<Card>();
            _currentDayTime = GameDayTime.Day; // Игра начинается с дневной фазы
            _dayNumber = 1;
            GameOver = false;
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

            while (!IsGameOver())
            {
                switch (_currentDayTime)
                {
                    case GameDayTime.Day:
                        DayPhase();
                        break;
                    case GameDayTime.Night:
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
            if (_dayNumber != 1)
            {
                GeneralDiscussion();
                IndividualDiscussion();
                Voting();

            }
            else
            {
                //Таймер на 10 секунд для ознакомления с картами
            }
            TransitionToPhase(GameDayTime.Night);
        }

        private void NightPhase()
        {
            if (_dayNumber != 1)
            {
                MafiaRoleplaying();
                SheriffRoleplaying();
                DoctorRoleplying();
            }
            else
            {
                MafiaAcquaintance();
            }
            TransitionToPhase(GameDayTime.Day);
        }

        private void IndividualDiscussion()
        {
            _currentGamePhase = GamePhase.IndividualDiscussionPhase;
            foreach (var player in players)
            {

            }
        }

        private void GeneralDiscussion()
        {
            _currentGamePhase = GamePhase.GeneralDiscussionPhase;

        }

        private void Voting()
        {
            _currentGamePhase = GamePhase.VotingPhase;

        }

        private void MafiaAcquaintance()
        {
            _currentGamePhase = GamePhase.MafiaAcquaintancePhase;
        }


        private void MafiaRoleplaying()
        {
            _currentGamePhase = GamePhase.MafiaPhase;
        }

        private void SheriffRoleplaying()
        {
            _currentGamePhase= GamePhase.SheriffPhase;
        }

        private void DoctorRoleplying()
        {
            _currentGamePhase = GamePhase.DoctorPhase;
        }


        private void TransitionToPhase(GameDayTime nextPhase)
        {
            // Переход между фазами
            _currentDayTime = nextPhase;
        }

        private bool IsGameOver()
        {
            if (_mafiaCount > _peacefulCount)
            {
                GameOver = false;
            }
            else
            {
                GameOver = true;
            }
            return GameOver;
        }
    }

    public enum GameDayTime
    {
        Day,
        Night
    }

    public enum GamePhase
    {
        MafiaAcquaintancePhase,
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

