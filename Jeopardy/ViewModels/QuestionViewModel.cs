using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Jeopardy.Helpers;
using Jeopardy.Models;
using Jeopardy.VMParameters;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace Jeopardy.ViewModels
{
    public class QuestionViewModel : BaseViewModel<QuestionVMParameter>
    {
        private QuestionVMParameter _vmParameter;

        public RoundItem Rounds { get; set; }
        public List<ContestantPointItem> ContestantsPoints { get; set; }

        public string Points { get; set; }
        public string CategoryName { get; set; }
        public string QuestionId { get; set; }
        public Question Question { get; set; }

        bool _showQuestionOnly;
        public bool ShowQuestionOnly
        {
            get { return _showQuestionOnly; }
            set
            {
                _showQuestionOnly = value;
                RaisePropertyChanged(() => ShowQuestionOnly);
            }
        }

        bool _showAnswer;
        public bool ShowAnswer
        {
            get { return _showAnswer; }
            set
            {
                _showAnswer = value;
                RaisePropertyChanged(() => ShowAnswer);
            }
        }

        bool _showScores;
        public bool ShowScores
        {
            get { return _showScores; }
            set
            {
                _showScores = value;
                RaisePropertyChanged(() => ShowScores);
            }
        }

        public ICommand SeeQuestionOnlyCommand { get; set; }
        public ICommand SeeAnswerCommand { get; set; }
        public ICommand SetScoresCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public QuestionViewModel(INavigation navigation, ILogger<QuestionViewModel> logger)
            : base(navigation, logger)
        {
            SeeQuestionOnlyCommand = new Command(SeeQuestionOnly);
            SeeAnswerCommand = new Command(SeeAnswer);
            SetScoresCommand = new Command(SetScores);
            BackCommand = new AsyncCommand(BackAsync);

            ContestantsPoints = new List<ContestantPointItem>();

            ShowQuestionOnly = true;
            ShowAnswer = false;
            ShowScores = false;
        }

        public override void Init(QuestionVMParameter initData)
        {
            _vmParameter = initData;

            Points = $"{initData.QuestionToCategory?.CategoryDifficulty?.Points.ToString()} Points";
            CategoryName = $"Category: {initData.QuestionToCategory?.Category?.Name}";
            QuestionId = $"ID: {initData.QuestionToCategory?.Question?.Id}";
            Question = initData.QuestionToCategory?.Question;

            ContestantsPoints.AddRange(initData?.PlayingGame?.ContestantsPoints
                ?.Select(cp => new ContestantPointItem
                {
                    CategoryDifficulty = initData?.QuestionToCategory?.CategoryDifficulty,
                    ContestantPoint = cp
                }));
        }

        void SeeQuestionOnly()
        {
            ShowQuestionOnly = true;
            ShowAnswer = false;
            ShowScores = false;
        }

        void SeeAnswer()
        {
            ShowQuestionOnly = false;
            ShowAnswer = true;
            ShowScores = false;
        }

        void SetScores()
        {
            ShowQuestionOnly = false;
            ShowAnswer = false;
            ShowScores = true;
        }

        async Task BackAsync()
        {
            await Navigation.PopAsync();
        }
    }
}
