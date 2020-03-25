using System;
using System.Collections.Generic;
using System.Windows.Input;
using Jeopardy.Data;
using Jeopardy.ViewModels;
using Xamarin.Forms;

namespace Jeopardy.Models
{
    public class PlayingGameItem
    {
        public PlayingGame PlayingGame { get; set; }

        public bool IsDeletable { get; set; }

        public string Title => PlayingGame?.Title + (IsDeletable ? " " + PlayingGame?.DateCreated?.LocalDateTime.ToString("M/d/yyyy H:mm:ss") : "");

        public ICommand DeleteCommand { get; set; }
    }

    public class RoundItem
    {
        public Round Round { get; set; }

        public ICommand ActionCommand { get; set; }
    }

    public class CategoryItem
    {
        public Category Category { get; set; }
        public IList<CategoryDifficultyItem> DifficultyItems { get; set; }

        public ICommand ActionCommand { get; set; }
    }

    public class CategoryDifficultyItem
    {
        public Category Category { get; set; }

        public CategoryDifficulty CategoryDifficulty { get; set; }

        public ICommand ActionCommand { get; set; }
    }

    public class ContestantPointItem : ExtendedBindableObject
    {
        public ContestantPoint ContestantPoint { get; set; }

        public CategoryDifficulty CategoryDifficulty { get; set; }

        public ICommand SetCorrectPointsCommand { get; set; }
        public ICommand SetWrongPointsCommand { get; set; }

        int _flag;
        public int Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                RaisePropertyChanged(() => ShowCorrectButton);
                RaisePropertyChanged(() => ShowWrongButton);
            }
        }

        public bool ShowCorrectButton => Flag <= 0;
        public bool ShowWrongButton => Flag >= 0;

        public ContestantPointItem()
        {
            SetCorrectPointsCommand = new Command(SetCorrectPoints);
            SetWrongPointsCommand = new Command(SetWrongPoints);

            Flag = 0;
        }

        void SetCorrectPoints()
        {
            if (ContestantPoint == null || CategoryDifficulty == null)
            {
                return;
            }

            DataRepository.UpdatePoints(ContestantPoint, ContestantPoint.Points + CategoryDifficulty.Points);
            Flag++;
        }

        void SetWrongPoints()
        {
            if (ContestantPoint == null || CategoryDifficulty == null)
            {
                return;
            }

            DataRepository.UpdatePoints(ContestantPoint, ContestantPoint.Points - CategoryDifficulty.Points);
            Flag--;
        }
    }
}
