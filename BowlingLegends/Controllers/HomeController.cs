using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using BowlingLegends.Models;
using BowlingLegends.OtherModels;

namespace BowlingLegends.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            var parameter = new SqlParameter("@season", 2016);
            var result = db.Database
                .SqlQuery<BowlingStats>("BowlingStatsTable @season", parameter)
                .ToList();

            //int series = result.Aggregate(0, (current, res) => current + res.Series);
            int series = db.Series.Count();

            var bowlingStats = SetCurrentRatings(result);
            bowlingStats = SetLastSeries(bowlingStats, series - 1);
            bowlingStats = SetPrediction(bowlingStats, series);

            //set rav
            foreach (var stat in bowlingStats)
            {
                foreach (var bowler in db.Bowlers)
                {
                    if (stat.BowlingID == bowler.BowlerID)
                        stat.RAV = (int)bowler.Scores.OrderByDescending(a => a.ScoreID).Take(4).Average(a => a.Score1);
                }
            }

            var finalDate = db.Seasons.OrderByDescending(x => x.Year).FirstOrDefault().FinalDate;
            int maxSeriesLeft = (int) Math.Ceiling((finalDate - DateTime.Now).TotalDays / 7);

            var viewModel = new HomeViewModel
            {
                BowlingStats = bowlingStats,
                PotMoney = series * db.Bowlers.Count(),
                ExpectedPotMoney = (series + maxSeriesLeft) * db.Bowlers.Count(),
                FinalDate = finalDate.AddDays(-2),
                MaxSeriesLeft = maxSeriesLeft,
                MaxPoints = maxSeriesLeft * 5

            };

            return View(viewModel);
        }

        private List<BowlingStats> SetPrediction(List<BowlingStats> bowlingStats, int series)
        {
            series = series - 5;
            if (series > 0)
            {
                var season = new SqlParameter("@season", 2016);
                var seriesid = new SqlParameter("@series", series);
                var result = db.Database
                    .SqlQuery<BowlingStats>("BowlingStatsSeriesTable @season, @series ", season, seriesid)
                    .ToList();
                var finalDate = db.Seasons.OrderByDescending(a => a.FinalDate).FirstOrDefault().FinalDate;
                foreach (var stat in bowlingStats)
                {
                    foreach (var res in result)
                    {
                        if (res.BowlingID == stat.BowlingID)
                        {
                            var earliestPoints = res.RankingPoints;
                            var avg = ((stat.RankingPoints - earliestPoints)/6.0);
                            var expectedAdditionalPoints = (int) (avg*(int) ((finalDate - db.Series.Where(a => a.SeriesID == (series + 5)).FirstOrDefault().Date).TotalDays/7));
                            stat.ExpectedTotalPoints = stat.RankingPoints + expectedAdditionalPoints;
                        }
                    }
                }
            }

            return bowlingStats;
        }

        private List<BowlingStats> SetLastSeries(List<BowlingStats> bowlingStats, int series)
        {
            var season = new SqlParameter("@season", 2016);
            var seriesid = new SqlParameter("@series", series);
            var result = db.Database
                    .SqlQuery<BowlingStats>("BowlingStatsSeriesTable @season, @series ", season, seriesid).ToList();

            result = SetCurrentRatings(result, series);

            var count = 1;
            foreach (var res in result)
            {
                res.LastWeekPosition = count;
                count++;
            }

            foreach (var bs in bowlingStats)
            {
                bs.LastWeekRating = result.Where(x => x.BowlingID == bs.BowlingID).Select(a => a.CurrentRating).FirstOrDefault();
                bs.LastWeekPoint = result.Where(x => x.BowlingID == bs.BowlingID).Select(a => a.RankingPoints).FirstOrDefault();
                bs.LastWeekPosition = result.Where(x => x.BowlingID == bs.BowlingID).Select(a => a.LastWeekPosition).FirstOrDefault();
            }
            return bowlingStats;
        }

        public ActionResult HistoricalTable()
        {
            var lastestSeries = db.Series.OrderByDescending(x => x.Date).FirstOrDefault().Date;
            var finalDate = db.Seasons.OrderByDescending(x => x.Year).FirstOrDefault().FinalDate;
            var overallList = new List<List<BowlingStats>>();
            var lastFewSeriesList = new List<List<BowlingStats>>();
            foreach (var se in db.Series)
            {
                var list = new List<BowlingStats>();
                var season = new SqlParameter("@season", 2016);
                var seriesid = new SqlParameter("@series", se.SeriesID);
                var result = db.Database
                    .SqlQuery<BowlingStats>("BowlingStatsSeriesTable @season, @series ", season, seriesid)
                    .ToList();

                list = SetCurrentRatings(result, se.SeriesID);
                list = SetPrediction(list, se.SeriesID);
                overallList.Add(list);
            }
            overallList.Reverse();
            lastFewSeriesList = overallList.GetRange(0, 6);

            var weeklyBowlingListList = new List<WeeklyBowlerStats>();
            foreach (var bowler in db.Bowlers)
            {
                var weeklyBowlingList = new WeeklyBowlerStats { Bowler = bowler };

                weeklyBowlingList.WeeksAtOne = overallList.Sum(a => a.Where(x => x.BowlingID == bowler.BowlerID).Sum(y => y.WeeksAtFirst));
                weeklyBowlingList.WeeksAtTwo = overallList.Sum(a => a.Where(x => x.BowlingID == bowler.BowlerID).Sum(y => y.WeeksAtSecond));
                weeklyBowlingList.WeeksAtThree = overallList.Sum(a => a.Where(x => x.BowlingID == bowler.BowlerID).Sum(y => y.WeeksAtThird));
                weeklyBowlingList.WeeksAtFour = overallList.Sum(a => a.Where(x => x.BowlingID == bowler.BowlerID).Sum(y => y.WeeksAtFourth));
                weeklyBowlingList.WeeksAtFive = overallList.Sum(a => a.Where(x => x.BowlingID == bowler.BowlerID).Sum(y => y.WeeksAtFifth));
                var latestPoints = lastFewSeriesList.Max(a => a.Where(x => x.BowlingID == bowler.BowlerID).Max(b => b.RankingPoints));
                weeklyBowlingList.CurrentPoints = latestPoints;
                var earliestPoints = lastFewSeriesList.Min(a => a.Where(x => x.BowlingID == bowler.BowlerID).Min(b => b.RankingPoints));
                var avg = ((latestPoints - earliestPoints) / 6.0);
                var expectedAdditionalPoints = avg * (int)((finalDate - lastestSeries).TotalDays / 7);
                weeklyBowlingList.AveragePointsLastFew = avg;
                weeklyBowlingList.ExpectedAdditionalPoints = (int)expectedAdditionalPoints;
                weeklyBowlingList.PredictedTotalScore = (int?)(latestPoints + expectedAdditionalPoints);


                weeklyBowlingListList.Add(weeklyBowlingList);
            }
            weeklyBowlingListList.Reverse();
            int maxSeriesLeft = (int)Math.Ceiling((finalDate - DateTime.Now).TotalDays / 7);

            var viewModel = new HistoricSeriesList
            {
                BowlingStatsList = overallList,
                WeeklyBowlingStatsList = weeklyBowlingListList,
                MaxSeriesLeft = maxSeriesLeft
            };
            return View(viewModel);
        }

        private List<BowlingStats> SetCurrentRatings(List<BowlingStats> bowlingStats)
        {
            foreach (var stat in bowlingStats)
            {
                var bowler = db.Bowlers.FirstOrDefault(x => x.BowlerID == stat.BowlingID);
                var placingList = new List<ScoresPlacing>();
                foreach (var se in db.Series)
                {
                    var bowlers = se.Rounds.SelectMany(x => x.Scores.Select(y => y.Bowler)).Distinct();
                    var bowlerSeriesScores = new Dictionary<Bowler, int>();
                    foreach (var b in bowlers)
                    {
                        var score = b.Scores.Where(x => x.Round.SeriesID == se.SeriesID).Select(x => x.Score1).Sum();
                        bowlerSeriesScores.Add(b, score);
                    }

                    bowlerSeriesScores = bowlerSeriesScores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    // var distinctScoresList = bowlerSeriesScores.Values.Distinct().ToList();

                    var scoreAndPlacingDictionary = new Dictionary<int, int>();
                    var peopleAhead = 0;

                    foreach (var ss in bowlerSeriesScores)
                    {
                        if (!scoreAndPlacingDictionary.ContainsKey(ss.Value))
                            scoreAndPlacingDictionary.Add(ss.Value, peopleAhead + 1);
                        peopleAhead++;
                    }
                    var currentScore = bowler.Scores.Where(x => x.Round.SeriesID == se.SeriesID).Select(x => x.Score1).Sum();
                    if (currentScore != 0)
                    {
                        var item = new ScoresPlacing
                        {
                            Id = se.SeriesID,
                            Score = currentScore,
                            Placing = scoreAndPlacingDictionary[currentScore]
                        };
                        placingList.Add(item);
                    }
                    else
                    {
                        var item = new ScoresPlacing
                        {
                            Id = se.SeriesID,
                            Score = null,
                            Placing = null
                        };
                        placingList.Add(item);
                    }

                }

                var seriesScoresAndPlacings = placingList;
                var lastFourSeries = seriesScoresAndPlacings.OrderByDescending(x => x.Id).Take(4);
                var currentRating = 0;
                foreach (var se in lastFourSeries)
                {
                    var placing = se.Placing;

                    switch (placing)
                    {
                        case 1:
                            currentRating = currentRating + 25;
                            break;
                        case 2:
                            currentRating = currentRating + 22;
                            break;
                        case 3:
                            currentRating = currentRating + 19;
                            break;
                        case 4:
                            currentRating = currentRating + 16;
                            break;
                        default:
                            currentRating = currentRating + 13;
                            break;
                    }
                }

                stat.CurrentRating = currentRating;
            }
            return bowlingStats;
        }

        private List<BowlingStats> SetCurrentRatings(List<BowlingStats> bowlingStats, int seriesId)
        {
            var count = 1;
            foreach (var stat in bowlingStats.OrderByDescending(x => x.RankingPoints))
            {
                switch (count)
                {
                    case 1:
                        stat.WeeksAtFirst = 1;
                        break;
                    case 2:
                        stat.WeeksAtSecond = 1;
                        break;
                    case 3:
                        stat.WeeksAtThird = 1;
                        break;
                    case 4:
                        stat.WeeksAtFourth = 1;
                        break;
                    case 5:
                        stat.WeeksAtFifth = 1;
                        break;
                }

                var bowler = db.Bowlers.FirstOrDefault(x => x.BowlerID == stat.BowlingID);
                var placingList = new List<ScoresPlacing>();
                foreach (var se in db.Series.Where(x => x.SeriesID <= seriesId))
                {
                    var bowlers = se.Rounds.SelectMany(x => x.Scores.Select(y => y.Bowler)).Distinct();
                    var bowlerSeriesScores = new Dictionary<Bowler, int>();
                    foreach (var b in bowlers)
                    {
                        var score = b.Scores.Where(x => x.Round.SeriesID == se.SeriesID).Select(x => x.Score1).Sum();
                        bowlerSeriesScores.Add(b, score);
                    }

                    bowlerSeriesScores = bowlerSeriesScores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    // var distinctScoresList = bowlerSeriesScores.Values.Distinct().ToList();

                    var scoreAndPlacingDictionary = new Dictionary<int, int>();
                    var peopleAhead = 0;

                    foreach (var ss in bowlerSeriesScores)
                    {
                        if (!scoreAndPlacingDictionary.ContainsKey(ss.Value))
                            scoreAndPlacingDictionary.Add(ss.Value, peopleAhead + 1);
                        peopleAhead++;
                    }
                    var currentScore = bowler.Scores.Where(x => x.Round.SeriesID == se.SeriesID).Select(x => x.Score1).Sum();
                    if (currentScore != 0)
                    {
                        var item = new ScoresPlacing
                        {
                            Id = se.SeriesID,
                            Score = currentScore,
                            Placing = scoreAndPlacingDictionary[currentScore]
                        };
                        placingList.Add(item);
                    }
                    else
                    {
                        var item = new ScoresPlacing
                        {
                            Id = se.SeriesID,
                            Score = null,
                            Placing = null
                        };
                        placingList.Add(item);
                    }

                }

                var seriesScoresAndPlacings = placingList;
                var lastFourSeries = seriesScoresAndPlacings.OrderByDescending(x => x.Id).Take(4);
                var currentRating = 0;
                foreach (var se in lastFourSeries)
                {
                    var placing = se.Placing;

                    switch (placing)
                    {
                        case 1:
                            currentRating = currentRating + 25;
                            break;
                        case 2:
                            currentRating = currentRating + 22;
                            break;
                        case 3:
                            currentRating = currentRating + 19;
                            break;
                        case 4:
                            currentRating = currentRating + 16;
                            break;
                        default:
                            currentRating = currentRating + 13;
                            break;
                    }
                }

                stat.CurrentRating = currentRating;
                count++;
            }

            return bowlingStats;
        }

        public ActionResult AllRounds()
        {
            var result = from s in db.Scores
                         join b in db.Bowlers on s.BowlerID equals b.BowlerID
                         join r in db.Rounds on s.RoundID equals r.RoundID
                         join ser in db.Series on r.SeriesID equals ser.SeriesID
                         join sea in db.Seasons on ser.SeasonID equals sea.SeasonID
                         orderby s.RoundID descending, s.Score1 descending
                         select new { sea.SeasonID, ser.SeriesID, b.Name, s.Score1, r.RoundID };

            var list = new List<RoundDetails>();
            foreach (var res in result)
            {
                var rd = new RoundDetails
                {
                    SeasonID = res.SeasonID,
                    SeriesID = res.SeriesID,
                    Name = res.Name,
                    Score = res.Score1,
                    RoundID = res.RoundID
                };
                list.Add(rd);
            }

            return View(list);
        }

        public JsonResult SeriesScoresGraph()
        {
            var seriesIds = db.Series.Select(x => x.SeriesID).ToList();
            var seriesScoresGraph = new List<SeriesScoresGraph>();
            var i = 1;
            foreach (var id in seriesIds)
            {
                var seriesScore = db.Scores.Where(x => x.Round.SeriesID == id).Select(x => x.Score1).Sum();
                var ssc = new SeriesScoresGraph
                {
                    Series = i,
                    Score = seriesScore
                };
                seriesScoresGraph.Add(ssc);
                i++;
            }
            return Json(seriesScoresGraph, JsonRequestBehavior.AllowGet);
        }

    }
}