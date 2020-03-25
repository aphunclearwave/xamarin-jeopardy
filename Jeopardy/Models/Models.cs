using System;
using System.Collections.Generic;
using Realms;

namespace Jeopardy.Models
{
    public enum RoundType
    {
        Round = 0,
        FinalRound = 1
    }

    public static class RoundTypeExtensions
    {
        public static string ToString(this RoundType roundType)
        {
            return ((int)roundType).ToString();
        }
    }

    public class PlayingGame : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Title { get; set; }

        public Game Game { get; set; }
        public IList<ContestantPoint> ContestantsPoints { get; }
        public bool Started { get; set; }

        public DateTimeOffset? DateCreated { get; set; }
    }

    public class Game : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Title { get; set; }

        public IList<Round> Rounds { get; }

        public IList<ContestantPoint> TeamPoints { get; }
    }

    public class Contestant : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Round : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }

        public IList<Category> Categories { get; }
    }

    public class Category : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }

        public IList<CategoryDifficulty> Difficulties { get; }
    }

    public class CategoryDifficulty : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public int LevelNumber { get; set; }

        public int Points { get; set; }

        public string RoundId { get; set; }
        public string Name { get; set; }
    }

    public class QuestionToCategory : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public Category Category { get; set; }
        public CategoryDifficulty CategoryDifficulty { get; set; }
        public Question Question { get; set; }
    }

    public class Question : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }

        public string Hint { get; set; }
        public string Explanation { get; set; }
    }

    public class ContestantPoint : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public Contestant Contestant { get; set; }

        public int Points { get; set; }
    }

}
