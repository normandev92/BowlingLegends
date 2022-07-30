namespace BowlingLegends.OtherModels
{
    public class BowlingStats
    {
        public int BowlingID { get; set; }
        public string Name { get; set; }
        public int Series { get; set; }
        public int Rounds { get; set; }
        public int SeriesWins { get; set; }
        public int SeriesSweeps { get; set; }
        public int SecondPlace { get; set; }
        public int ThirdPlace { get; set; }
        public int LastPlace { get; set; }
        public int OneHundredFifty { get; set; }
        public int ThreeHundred { get; set; }
        public int RoundWins { get; set; }
        public int TotalPoints { get; set; }
        public int AveragePoints { get; set; }
        public int LowScore { get; set; }
        public int LowestSeriesScore { get; set; }
        public int HighScore { get; set; }
        public int HighestSeriesScore { get; set; }
        public int RankingPoints { get; set; }
        public int? CurrentRating { get; set; }
        public int? LastWeekRating { get; set; }
        public int? LastWeekPoint { get; set; }
        public int? LastWeekPosition { get; set; }
        public int? WeeksAtFirst { get; set; }
        public int? WeeksAtSecond { get; set; }
        public int? WeeksAtThird { get; set; }
        public int? WeeksAtFourth { get; set; }
        public int? WeeksAtFifth { get; set; }
        public int ExpectedTotalPoints { get; set; }
        public int RAV { get; set; }
    }
}