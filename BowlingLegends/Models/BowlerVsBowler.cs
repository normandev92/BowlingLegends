using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BowlingLegends.Models
{
    public class BowlerVsBowler
    {
        public Bowler Bowler { get; set; }
        public int SeriesWin { get; set; }
        public int SeriesLoss { get; set; }
        public int SeriesdDraw { get; set; }
        public int RoundWin { get; set; }
        public int RoundLoss { get; set; }
        public int RoundDraw { get; set; }
    }
}