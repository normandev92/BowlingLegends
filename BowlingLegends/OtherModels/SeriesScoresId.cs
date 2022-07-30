using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BowlingLegends.Models;

namespace BowlingLegends.OtherModels
{
    public class SeriesScoresId
    {
        public int Series { get; set; }
        public Bowler Bowler { get; set; }
        public int Score { get; set; }
    }
}