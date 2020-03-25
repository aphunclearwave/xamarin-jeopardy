using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeopardy.Models;
using Realms;

namespace Jeopardy.Data
{
    public static class DataRepository
    {
        public static IQueryable<Game> GetAllGames()
        {
            try
            {
                Realm realm = Realm.GetInstance();
                realm.Refresh();
                var games = realm.All<Game>();
                return games;
            }
            catch (Exception ex)
            {

            }
            return new List<Game>().AsQueryable();
        }

        public static void Cleanup()
        {
            Realm realm = Realm.GetInstance();
            realm.Refresh();
            try
            {
                realm.Write(() =>
                {
                    realm.RemoveRange(realm.All<PlayingGame>().Where(g => g.Started == false && g.DateCreated != null));
                });
            }
            catch (Exception ex)
            {

            }
        }

        public static IQueryable<PlayingGame> GetAllPlayingGames(bool valid = true)
        {
            Realm realm = Realm.GetInstance();
            realm.Refresh();
            var playingGames = realm.All<PlayingGame>();
            if (valid)
            {
                playingGames = playingGames.Where(g => g.Started == true && g.DateCreated != null);
            }
            return playingGames;
        }

        public static IQueryable<Contestant> GetAllContestants()
        {
            Realm realm = Realm.GetInstance();
            var contestants = realm.All<Contestant>();
            return contestants;
        }

        public static PlayingGame NewPlayingGame(Game game)
        {
            var newPlayingGame = new PlayingGame
            {
                Title = $"New Game: {game.Title}",
                Game = game,
                Id = Guid.NewGuid().ToString("D")
            };

            var allContestants = GetAllContestants();
            foreach (var contestant in allContestants)
            {
                var cPoint = new ContestantPoint
                {
                    Id = Guid.NewGuid().ToString("D"),
                    Contestant = contestant
                };
                newPlayingGame.ContestantsPoints.Add(cPoint);
            }

            Realm realm = Realm.GetInstance();
            realm.Write(() =>
            {
                realm.Add(newPlayingGame);
            });

            return newPlayingGame;
        }

        public static async Task PlayGame(PlayingGame game)
        {
            try
            {
                Realm realm = Realm.GetInstance();
                realm.Write(() =>
                {
                    game.Started = true;
                    game.DateCreated = DateTimeOffset.Now;
                    realm.Add(game, true);
                });
            }
            catch (Exception ex)
            {

            }
        }

        public static QuestionToCategory GetQuestionToCategory(Category category, CategoryDifficulty categoryDifficulty)
        {
            if (category == null)
            {
                return null;
            }

            Realm realm = Realm.GetInstance();
            return realm.All<QuestionToCategory>()
                .FirstOrDefault(i => i.Category == category &&
                                i.CategoryDifficulty == categoryDifficulty);
        }

        public static bool RemoveGame(PlayingGame game)
        {
            if (game == null)
            {
                return false;
            }

            try
            {
                Realm realm = Realm.GetInstance();
                realm.Refresh();
                var game2 = realm.All<PlayingGame>().FirstOrDefault(g => g.Id == game.Id);
                if (game2 != null)
                {
                    realm.Write(() =>
                    {

                        realm.Remove(game2);

                    });
                }
                realm.Write(() =>
                {
                    realm.Remove(game);
                });

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public static void UpdatePoints(ContestantPoint contestantPoint, int newPoints)
        {
            if (contestantPoint == null)
            {
                return;
            }

            try
            {
                Realm realm = Realm.GetInstance();
                realm.Refresh();
                realm.Write(() =>
                {
                    contestantPoint.Points = newPoints;
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
