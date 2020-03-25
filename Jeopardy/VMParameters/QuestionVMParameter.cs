using Jeopardy.Models;

namespace Jeopardy.VMParameters
{
    public class QuestionVMParameter
    {
        public PlayingGame PlayingGame { get; private set; }
        public RoundItem RoundItem { get; private set; }

        public QuestionToCategory QuestionToCategory { get; private set; }

        public QuestionVMParameter(
            PlayingGame playingGame,
            RoundItem roundItem,
            QuestionToCategory questionToCategory)
        {
            this.PlayingGame = playingGame;
            this.RoundItem = roundItem;
            this.QuestionToCategory = questionToCategory;
        }
    }
}
