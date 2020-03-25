using System;
using Jeopardy.Models;

namespace Jeopardy.VMParameters
{
    public class GameVMParameter
    {
        public PlayingGame PlayingGame { get; private set; }

        public GameVMParameter(PlayingGame playingGame)
        {
            this.PlayingGame = playingGame;
        }
    }
}
