using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Jeopardy.Data;
using Jeopardy.Helpers;
using Jeopardy.Models;
using Jeopardy.Pages;
using Jeopardy.VMParameters;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace Jeopardy.ViewModels
{
    public class RoundsViewModel : BaseViewModel<RoundsVMParameter>
    {
        private PlayingGame _playingGame;

        public string GameTitle => _playingGame?.Title;

        public List<RoundItem> Rounds { get; set; }
        public List<ContestantPoint> ContestantsPoints { get; set; }

        ObservableCollection<CategoryItem> _categoryItems;
        public ObservableCollection<CategoryItem> CategoryItems
        {
            get { return _categoryItems; }
            set
            {
                _categoryItems = value;
                RaisePropertyChanged(() => CategoryItems);
            }
        }

        RoundItem _selectedRoundItem;
        public RoundItem SelectedRoundItem
        {
            get { return _selectedRoundItem; }
            set
            {
                _selectedRoundItem = value;
                RaisePropertyChanged(() => SelectedRoundItem);
            }
        }

        AsyncCommand<RoundItem> _goToRoundCommand;
        public AsyncCommand<RoundItem> GoToRoundCommand
        {
            get { return _goToRoundCommand; }
            set
            {
                _goToRoundCommand = value;
                RaisePropertyChanged(() => GoToRoundCommand);
            }
        }

        AsyncCommand<CategoryDifficultyItem> _goToQuestionCommand;
        public AsyncCommand<CategoryDifficultyItem> GoToQuestionCommand
        {
            get { return _goToQuestionCommand; }
            set
            {
                _goToQuestionCommand = value;
                RaisePropertyChanged(() => GoToQuestionCommand);
            }
        }

        public RoundsViewModel(INavigation navigation, ILogger<ChooseGameViewModel> logger)
            : base(navigation, logger)
        {
            GoToRoundCommand = new AsyncCommand<RoundItem>(OnGoToRoundAsync);
            GoToQuestionCommand = new AsyncCommand<CategoryDifficultyItem>(GoToQuestionAsync);

            ContestantsPoints = new List<ContestantPoint>();
            Rounds = new List<RoundItem>();
            CategoryItems = new ObservableCollection<CategoryItem>();
        }

        public override void Init(RoundsVMParameter initData)
        {
            _playingGame = initData.PlayingGame;

            ContestantsPoints.AddRange(initData?.PlayingGame?.ContestantsPoints);
            Rounds.AddRange(_playingGame?.Game?.Rounds?.Select(r => new RoundItem { Round = r, ActionCommand = this.GoToRoundCommand } ));
        }

        async Task OnGoToRoundAsync(RoundItem roundItem)
        {
            if (roundItem == null)
            {
                return;
            }

            if (roundItem.Round?.Categories == null)
            {
                return;
            }

            SelectedRoundItem = roundItem;

            // create a new structure
            // so GoToQuestionAsync() has the category data
            CategoryItems = new ObservableCollection<CategoryItem>(roundItem.Round.Categories
                .Select(c => new CategoryItem
                {
                    Category = c,
                    DifficultyItems = c.Difficulties.Select(d => new CategoryDifficultyItem
                    {
                        Category = c,
                        CategoryDifficulty = d,
                        ActionCommand = GoToQuestionCommand
                    }).ToList()
                }));
        }

        async Task GoToQuestionAsync(CategoryDifficultyItem categoryDifficultyItem)
        {
            if (categoryDifficultyItem == null)
            {
                return;
            }

            var question2Category = DataRepository.GetQuestionToCategory(categoryDifficultyItem.Category,
                categoryDifficultyItem.CategoryDifficulty);

            var questionParam = new QuestionVMParameter(_playingGame, _selectedRoundItem, question2Category);

            await NavigateToPage(new QuestionPage(questionParam));
        }
    }
}
