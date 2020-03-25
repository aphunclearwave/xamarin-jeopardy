using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class GameViewModel : BaseViewModel<GameVMParameter>
    {
        private PlayingGame _playingGame;

        public string GameTitle => _playingGame?.Title;
        public ObservableCollection<ContestantPoint> ContestantsPoints { get; set; }

        public ICommand PlayGameCommand { get; set; }


        public GameViewModel(INavigation navigation, ILogger<ChooseGameViewModel> logger)
            : base(navigation, logger)
        {
            PlayGameCommand = new AsyncCommand(OnPlayGameAsync);

            ContestantsPoints = new ObservableCollection<ContestantPoint>();
        }

        public override void Init(GameVMParameter initData)
        {
            _playingGame = initData.PlayingGame;

            ContestantsPoints = new ObservableCollection<ContestantPoint>(initData?.PlayingGame?.ContestantsPoints);
        }

        async Task OnPlayGameAsync()
        {
            await DataRepository.PlayGame(_playingGame);

            App.Current.MainPage = new NavigationPage(new RoundsPage(new RoundsVMParameter(_playingGame)));
            //await NavigateToPage();
        }
    }
}
