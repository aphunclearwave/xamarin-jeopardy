using Jeopardy.Models;

namespace Jeopardy.VMParameters
{
    public class RoundsVMParameter
    {
        public PlayingGame PlayingGame { get; private set; }

        public RoundsVMParameter(PlayingGame playingGame)
        {
            this.PlayingGame = playingGame;
        }
    }
}
