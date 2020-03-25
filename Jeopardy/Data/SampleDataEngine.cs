using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Jeopardy.Models;
using Newtonsoft.Json;
using Realms;

namespace Jeopardy.Data
{
    public class SampleDataEngine
    {
        public async static Task SampleDataToLocal(SampleDataRoot sampleData, bool deleteAll = false)
        {
            if (sampleData == null || sampleData.Games == null)
            {
                return;
            }

            Realm realm = Realm.GetInstance();
            bool hasAnyItems = false;
            try
            {
                realm.Refresh();
                hasAnyItems = realm.All<Game>().Any();
            }
            catch (Exception ex)
            {

            }

            if (deleteAll)
            {
                try
                {
                    await realm.WriteAsync(r =>
                    {
                        r.RemoveAll();
                    });
                }
                catch (Exception ex)
                {

                }
            }

            if (!hasAnyItems)
            {
                foreach (var game in sampleData.Games)
                {
                    var newGame = new Game()
                    {
                        Id = game.ToString(),
                        Title = game.Title
                    };

                    // category points
                    var categoryPoints = new List<CategoryDifficulty>();
                    foreach (var categoryPointId in game.CategoriesPointsIds)
                    {
                        var categoryPointData = sampleData.CategoriesPoints?.FirstOrDefault(c => c.Id == categoryPointId);
                        if (categoryPointData != null)
                        {
                            var newCategoryDifficulty = new CategoryDifficulty
                            {
                                Id = categoryPointId.ToString(),
                                LevelNumber = categoryPointData.LevelNumber,
                                Points = categoryPointData.Points,
                                Name = categoryPointData.Name,
                                RoundId = categoryPointData.RoundId.ToString()
                            };

                            categoryPoints.Add(newCategoryDifficulty);

                            try
                            {
                                await realm.WriteAsync((r) =>
                                {
                                    r.Add(newCategoryDifficulty);
                                });
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    // rounds
                    foreach (var roundId in game.RoundsIds)
                    {
                        var roundData = sampleData.Rounds?.FirstOrDefault(r => r.Id == roundId);
                        if (roundData != null)
                        {
                            var newRound = new Round
                            {
                                Id = roundId.ToString(),
                                Name = roundData.Name
                            };

                            if (roundData.CategoriesIds != null)
                            {
                                // categories for a round
                                foreach (var categoryId in roundData.CategoriesIds)
                                {
                                    var categoryData = sampleData.Categories?.FirstOrDefault(c => c.Id == categoryId);
                                    if (categoryData != null)
                                    {
                                        var newCategory = new Category
                                        {
                                            Id = categoryId.ToString(),
                                            Name = categoryData.Name
                                        };

                                        var categoryPointsForRound = categoryPoints.Where(cp => cp.RoundId == newRound.Id).OrderBy(cp => cp.LevelNumber);
                                        foreach (var categoryPointForRound in categoryPointsForRound)
                                        {
                                            newCategory.Difficulties.Add(categoryPointForRound);
                                        }
                                        //TODO: do not save difficulty on category, just get it form round data every single time
                                        newRound.Categories.Add(newCategory);
                                    }
                                }
                            }

                            newGame.Rounds.Add(newRound);
                        }
                    }

                    // Update and persist objects with a thread-safe transaction
                    await realm.WriteAsync((r) =>
                    {
                        r.Add(newGame);
                    });

                    // teams
                    if (game.TeamsIds != null)
                    {
                        await realm.WriteAsync((r) =>
                        {
                            foreach (var teamId in game.TeamsIds)
                            {
                                var teamData = sampleData.Teams?.FirstOrDefault(t => t.Id == teamId);
                                if (teamData != null)
                                {
                                    var contestant = new Contestant
                                    {
                                        Id = teamId.ToString(),
                                        Name = teamData.Name
                                    };
                                    r.Add(contestant);
                                }
                            }
                        });
                    }

                    // questions
                    if (sampleData.Questions != null)
                    {
                        foreach (var question in sampleData.Questions)
                        {
                            var newQuestion = new Question
                            {
                                QuestionText = question.Question,
                                AnswerText = question.Answer,
                                Id = question.Id.ToString(),
                                Hint = question.Hints,
                                Explanation = question.Explanation
                            };

                            await realm.WriteAsync((r) =>
                            {
                                r.Add(newQuestion);
                            });
                        }
                    }

                    // link questions to categories
                    if (sampleData.Questions2Categories != null)
                    {
                        foreach (var question2Category in sampleData.Questions2Categories)
                        {
                            var question = realm.All<Question>().FirstOrDefault(q => q.Id == question2Category.QuestionId.ToString());
                            var category = realm.All<Category>().FirstOrDefault(c => c.Id == question2Category.CategoryId.ToString());
                            var categoryDifficulty = realm.All<CategoryDifficulty>().FirstOrDefault(d => d.Id == question2Category.CategoryPointsId.ToString());

                            var newQuestionToCategory = new QuestionToCategory
                            {
                                Id = question2Category.Id.ToString(),
                                //Game = game,
                                Question = question,
                                Category = category,
                                CategoryDifficulty = categoryDifficulty
                            };

                            await realm.WriteAsync((r) =>
                            {
                                r.Add(newQuestionToCategory);
                            });
                        }
                    }
                }
            }
        }

        public async static Task<SampleDataRoot> GetSampleData()
        {
            var assembly = typeof(SampleDataEngine).GetTypeInfo().Assembly;
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith("sample_data.json", StringComparison.InvariantCulture))
                {
                    try
                    {
                        string filecontent = await GetJsonFromResourceName(assembly, resourceName);

                        var sampleData = GetDataFromJson<SampleDataRoot>(filecontent);
                        return sampleData;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return null;
        }

        private static TData GetDataFromJson<TData>(string jsonContent)
        {
            return JsonConvert.DeserializeObject<TData>(jsonContent);
        }

        private static async Task<string> GetJsonFromResourceName(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = await reader.ReadToEndAsync();
                return json;
            }
        }
    }

    public class SampleDataRoot
    {
        public SampleDataGame[] Games { get; set; }
        public SampleDataTeam[] Teams { get; set; }

        [JsonProperty("team_points")]
        public SampleDataTeamPoint[] TeamsPoints { get; set; }

        public SampleDataRound[] Rounds { get; set; }
        public SampleDataCategories[] Categories { get; set; }

        [JsonProperty("categories_difficulties")]
        public SampleDataCategoriesDifficulties[] CategoriesDifficulties { get; set; }

        [JsonProperty("categories_points")]
        public SampleDataCategoryPoint[] CategoriesPoints { get; set; }

        [JsonProperty("questions")]
        public SampleDataQuestion[] Questions { get; set; }

        [JsonProperty("questions_to_categories")]
        public SampleDataQuestionToCategory[] Questions2Categories { get; set; }
    }

    public class SampleDataGame
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [JsonProperty("teams")]
        public int[] TeamsIds { get; set; }

        [JsonProperty("rounds")]
        public int[] RoundsIds { get; set; }

        [JsonProperty("categories_difficulties")]
        public int[] CategoryDifficultiesIds { get; set; }

        [JsonProperty("categories_points")]
        public int[] CategoriesPointsIds { get; set; }

        [JsonProperty("team_points")]
        public int[] TeamPointsIds { get; set; }
    }

    public class SampleDataTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SampleDataTeamPoint
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int TeamId { get; set; }
        public int Points { get; set; }
        public int Wage { get; set; }
        public string Color { get; set; }
    }

    public class SampleDataRound
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsFinal { get; set; }

        [JsonProperty("categories")]
        public int[] CategoriesIds { get; set; }
    }

    public class SampleDataCategories
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // not used
    public class SampleDataCategoriesDifficulties
    {
        public int Id { get; set; }

        [JsonProperty("level_number")]
        public int LevelNumber { get; set; }

        [JsonProperty("round")]
        public int RoundId { get; set; }
        public int Points { get; set; }
        public string Name { get; set; }
    }

    public class SampleDataCategoryPoint
    {
        public int Id { get; set; }

        [JsonProperty("level_number")]
        public int LevelNumber { get; set; }

        [JsonProperty("round")]
        public int RoundId { get; set; }

        public int Points { get; set; }
        public string Name { get; set; }
    }

    public class SampleDataQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Hints { get; set; }
        public string Explanation { get; set; }
    }

    public class SampleDataQuestionToCategory
    {
        public int Id { get; set; }

        [JsonProperty("category_id")]
        public int CategoryId { get; set; }

        [JsonProperty("category_points_id")]
        public int CategoryPointsId { get; set; }

        [JsonProperty("question_id")]
        public int QuestionId { get; set; }
    }
}
