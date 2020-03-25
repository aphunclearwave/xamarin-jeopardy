using System;
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
    public class ChooseGameViewModel : BaseViewModel
    {
        ObservableCollection<PlayingGameItem> _gameItems;
        public ObservableCollection<PlayingGameItem> GameItems
        {
            get { return _gameItems; }
            set
            {
                _gameItems = value;
                RaisePropertyChanged(() => GameItems);
            }
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        public ICommand SelectGameCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        ICommand DeleteCommand { get; set; }

        public ChooseGameViewModel(INavigation navigation, ILogger<ChooseGameViewModel> logger)
            : base(navigation, logger)
        {
            SelectGameCommand = new AsyncCommand<PlayingGameItem>(OnSelectGame);
            RefreshCommand = new AsyncCommand(OnRefreshAsync, () => !IsRefreshing);
            DeleteCommand = new AsyncCommand<PlayingGameItem>(OnDeleteGameAsync);
        }

        public override void Init()
        {
            GetItems();
        }

        void GetItems()
        {
            var allGameTypes = DataRepository.GetAllGames();
            var playingGameItems = new List<PlayingGameItem>();

            foreach (var gameType in allGameTypes)
            {
                var item = DataRepository.NewPlayingGame(gameType);
                playingGameItems.Add(new PlayingGameItem
                {
                    PlayingGame = item
                });
            }

            var savedPlayingGames = DataRepository.GetAllPlayingGames();
            if (savedPlayingGames.Count() > 0)
            {
                playingGameItems.AddRange(savedPlayingGames.OrderByDescending(g => g.DateCreated).ToList().Select(g => new PlayingGameItem
                {
                    PlayingGame = g,
                    IsDeletable = true,
                    DeleteCommand = this.DeleteCommand
                }));
            }
            GameItems = new ObservableCollection<PlayingGameItem>(playingGameItems);
        }

        async Task OnSelectGame(PlayingGameItem game)
        {
            if (game == null || game.PlayingGame == null)
            {
                return;
            }

            await NavigateToPage(new GamePage(new GameVMParameter(game.PlayingGame)));
        }

        async Task OnRefreshAsync()
        {
            await Task.Delay(500);

            GetItems();
            IsRefreshing = false;
        }

        async Task OnDeleteGameAsync(PlayingGameItem game)
        {
            if (game == null || game.PlayingGame == null)
            {
                return;
            }

            this.GameItems.Clear();

            var success = this.GameItems.Remove(game);
            if (!success)
            {
                var game2 = this.GameItems.FirstOrDefault(i => i.PlayingGame.Id == game.PlayingGame.Id);
                success = this.GameItems.Remove(game2);
            }
            DataRepository.RemoveGame(game.PlayingGame);

            IsRefreshing = true;
            await OnRefreshAsync();
        }
    }
}
