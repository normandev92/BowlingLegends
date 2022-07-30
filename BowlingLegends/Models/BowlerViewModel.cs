using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BowlingLegends.OtherModels;

namespace BowlingLegends.Models
{
    public class BowlerViewModel
    {
        public Bowler Bowler { get; set; }
        public int Rounds { get; set; }
        public int TotalSeries { get; set; }
        public List<int> RoundScores { get; set; }
        public List<int> SeriesIds { get; set; }
        public List<int> SeriesScores { get; set; }
        public List<ScoresPlacing> SeriesScoresAndPlacings { get; set; }
        public List<ScoresPlacing> RoundScoresAndPlacings { get; set; }
        public List<SeriesScoresGraph> SeriesScoresGraph { get; set; }
        public List<RoundScoresGraph> RoundScoresGraph { get; set; }
        public int RankingPoints { get; set; }
        public int RondWins { get; set; }
        public int SeriesWins { get; set; }
        public int SeriesSweeps { get; set; }
        public int Seconds { get; set; }
        public int Thirds { get; set; }
        public int Lasts { get; set; }
        public int HighestRoundScore { get; set; }
        public int LowestRoundScore { get; set; }
        public int HighestSeriesScore { get; set; }
        public double Average { get; set; }
        public int Range { get; set; }
        public int OneHundredFifty { get; set; }
        public int ThreeHundred { get; set; }
        public int LowestSeriesScore { get; set; }
        public int TotalPins { get; set; }
        public int CurrentRating { get; set; }
        public List<BowlerVsBowler> BowlerVsBowlersList { get; set; }

        public BowlingLegendsContext _db { get; set; }

        public BowlerViewModel(BowlingLegendsContext db, Bowler bowler)
        {
            _db = db;
            Bowler = bowler;

        }

        public void Update()
        {
            RoundScores = new List<int>(Bowler.Scores.OrderBy(x => x.RoundID).Select(x => x.Score1));
            HighestRoundScore = Bowler.Scores.Max(x => x.Score1);
            LowestRoundScore = Bowler.Scores.Min(x => x.Score1);
            TotalPins = Bowler.Scores.Sum(x => x.Score1);
            Average = Bowler.Scores.Average(x => x.Score1);
            Range = HighestRoundScore - LowestRoundScore;
            Rounds = Bowler.Scores.Count;
            SeriesIds = Bowler.Scores.ToList().Select(x => x.Round).Select(x => x.Series).Select(x => x.SeriesID).Distinct().ToList();
            TotalSeries = SeriesIds.Count;

            SeriesScores = new List<int>();
            foreach (var id in SeriesIds)
            {
                var seriesScore = Bowler.Scores.Where(x => x.Round.SeriesID == id).Select(x => x.Score1).Sum();
                SeriesScores.Add(seriesScore);
            }

            HighestSeriesScore = SeriesScores.Max();
            LowestSeriesScore = SeriesScores.Min();

            OneHundredFifty = RoundScores.Count(x => x >= 150);
            ThreeHundred = SeriesScores.Count(x => x >= 300);



            var parameter = new SqlParameter("@bowlerId", Bowler.BowlerID);
            var result = _db.Database
                .SqlQuery<BowlerStats>("BowlerStatsTable @bowlerId", parameter).FirstOrDefault();
            if (result != null)
            {
                RankingPoints = result.RankingPoints;
                RondWins = result.RondWins;
                SeriesWins = result.SeriesWins;
                SeriesSweeps = result.SeriesSweeps;
                Seconds = result.Seconds;
                Thirds = result.Thirds;
                Lasts = result.Lasts;
            }
            SetSeriesScorePlacing();
            SetRoundScorePlacing();
            var lastFourSeries = SeriesScoresAndPlacings.OrderByDescending(x => x.Id).Take(4);
            var currentRating = 0;
            foreach (var series in lastFourSeries)
            {
                var placing = series.Placing;

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

            CurrentRating = currentRating;


            SetBowlerVsBowler();
        }

        private void SetBowlerVsBowler()
        {
            var bvbList = new List<BowlerVsBowler>();
            var bowlers = _db.Bowlers.Where(x => x.BowlerID != Bowler.BowlerID);
            var series = _db.Series;
            var rounds = _db.Rounds;
            foreach (var bowler in bowlers)
            {
                var bvb = new BowlerVsBowler { Bowler = bowler };
                foreach (var serie in series)
                {
                    var theirScore = bowler.Scores.Where(x => x.Round.SeriesID == serie.SeriesID).Select(x => x.Score1).Sum();
                    var myScore = Bowler.Scores.Where(x => x.Round.SeriesID == serie.SeriesID).Select(x => x.Score1).Sum();
                    if(theirScore == 0 | myScore == 0)
                        continue;
                    if (theirScore < myScore)
                        bvb.SeriesWin++;
                    else if (myScore < theirScore)
                        bvb.SeriesLoss++;
                    else
                        bvb.SeriesdDraw++;
                }

                foreach (var round in rounds)
                {
                    var theirScore = bowler.Scores.Where(x => x.Round.RoundID == round.RoundID).Select(x => x.Score1).FirstOrDefault();
                    var myScore = Bowler.Scores.Where(x => x.Round.RoundID == round.RoundID).Select(x => x.Score1).FirstOrDefault();
                    if (theirScore == 0 | myScore == 0)
                        continue;
                    if (theirScore < myScore)
                        bvb.RoundWin++;
                    else if (myScore < theirScore)
                        bvb.RoundLoss++;
                    else
                        bvb.RoundDraw++;
                }

                bvbList.Add(bvb);
            }
            BowlerVsBowlersList = bvbList;
        }

        private void SetRoundScorePlacing()
        {
            var placingList = new List<ScoresPlacing>();
            foreach (var round in _db.Rounds)
            {
                var bowlers = round.Scores.Select(y => y.Bowler).Distinct();
                var bowlerRoundScores = new Dictionary<Bowler, int>();
                foreach (var b in bowlers)
                {
                    var score = b.Scores.Where(x => x.Round.RoundID == round.RoundID).Select(x => x.Score1).FirstOrDefault();
                    bowlerRoundScores.Add(b, score);
                }

                bowlerRoundScores = bowlerRoundScores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                // var distinctScoresList = bowlerSeriesScores.Values.Distinct().ToList();

                var scoreAndPlacingDictionary = new Dictionary<int, int>();
                var peopleAhead = 0;

                foreach (var ss in bowlerRoundScores)
                {
                    if (!scoreAndPlacingDictionary.ContainsKey(ss.Value))
                        scoreAndPlacingDictionary.Add(ss.Value, peopleAhead + 1);
                    peopleAhead++;
                }
                var currentScore = Bowler.Scores.Where(x => x.Round.RoundID == round.RoundID).Select(x => x.Score1).FirstOrDefault();
                if (currentScore != 0)
                {
                    var item = new ScoresPlacing
                    {
                        Id = round.RoundID,
                        Score = currentScore,
                        Placing = scoreAndPlacingDictionary[currentScore]
                    };
                    placingList.Add(item);
                }
                else
                {
                    var item = new ScoresPlacing
                    {
                        Id = round.RoundID,
                        Score = null,
                        Placing = null
                    };
                    placingList.Add(item);
                }

            }
            RoundScoresAndPlacings = placingList;

        }

        private void SetSeriesScorePlacing()
        {
            var placingList = new List<ScoresPlacing>();
            foreach (var series in _db.Series)
            {
                var bowlers = series.Rounds.SelectMany(x => x.Scores.Select(y => y.Bowler)).Distinct();
                var bowlerSeriesScores = new Dictionary<Bowler, int>();
                foreach (var b in bowlers)
                {
                    var score = b.Scores.Where(x => x.Round.SeriesID == series.SeriesID).Select(x => x.Score1).Sum();
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
                var currentScore = Bowler.Scores.Where(x => x.Round.SeriesID == series.SeriesID).Select(x => x.Score1).Sum();
                if (currentScore != 0)
                {
                    var item = new ScoresPlacing
                    {
                        Id = series.SeriesID,
                        Score = currentScore,
                        Placing = scoreAndPlacingDictionary[currentScore]
                    };
                    placingList.Add(item);
                }
                else
                {
                    var item = new ScoresPlacing
                    {
                        Id = series.SeriesID,
                        Score = null,
                        Placing = null
                    };
                    placingList.Add(item);
                }

            }
            SeriesScoresAndPlacings = placingList;
        }

        public void SetSeriesScoresGraph()
        {
            SeriesIds = Bowler.Scores.ToList().Select(x => x.Round).Select(x => x.Series).Select(x => x.SeriesID).Distinct().ToList();
            SeriesScores = new List<int>();
            SeriesScoresGraph = new List<SeriesScoresGraph>();
            var i = 1;
            foreach (var id in SeriesIds)
            {
                var seriesScore = Bowler.Scores.Where(x => x.Round.SeriesID == id).Select(x => x.Score1).Sum();
                SeriesScores.Add(seriesScore);
                var ssc = new SeriesScoresGraph
                {
                    Series = i,
                    Score = seriesScore
                };
                SeriesScoresGraph.Add(ssc);
                i++;
            }
        }

        public void SetRoundScoresGraph()
        {
            RoundScores = new List<int>(Bowler.Scores.OrderBy(x => x.RoundID).Select(x => x.Score1));
            RoundScoresGraph = new List<RoundScoresGraph>();
            var j = 1;
            foreach (var rc in RoundScores)
            {
                var rsg = new RoundScoresGraph
                {
                    Score = rc,
                    Round = j
                };
                RoundScoresGraph.Add(rsg);
                j++;
            }
        }
    }

    public class BowlerStats
    {
        public int RankingPoints { get; set; }
        public int RondWins { get; set; }
        public int SeriesWins { get; set; }
        public int SeriesSweeps { get; set; }
        public int Seconds { get; set; }
        public int Thirds { get; set; }
        public int Lasts { get; set; }
    }

    public class WeeklyBowlerStats
    {
        public Bowler Bowler { get; set; }
        public int? WeeksAtOne { get; set; }
        public int? WeeksAtTwo { get; set; }
        public int? WeeksAtThree { get; set; }
        public int? WeeksAtFour { get; set; }
        public int? WeeksAtFive { get; set; }
        public int? PredictedTotalScore { get; set; }
        public double AveragePointsLastFew { get; set; }
        public int ExpectedAdditionalPoints { get; set; }
        public int CurrentPoints { get; set; }
    }
}