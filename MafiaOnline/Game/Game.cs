using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.Game
{
    internal class Game
    {
        private Dictionary<Player, TcpClient> connections;
        private GameState state;
        private GamePhase currentPhase;

        public Game()
        {
            
            currentPhase = GamePhase.Day; // Игра начинается с дневной фазы
        }

        public void Start()
        {
            while (!IsGameOver())
            {
                switch (currentPhase)
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

        private Dictionary<Player,TcpClient> randomizeRoles()
        {

            return connections;
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

        private void TransitionToPhase(GamePhase nextPhase)
        {
            // Переход между фазами
            currentPhase = nextPhase;
        }

        private bool IsGameOver()
        {
            // Проверка условий окончания игры
            // ...
            return false;
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

