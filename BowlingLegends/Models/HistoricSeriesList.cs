using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BowlingLegends.OtherModels;

namespace BowlingLegends.Models
{
    public class HistoricSeriesList
    {
        public List <List<BowlingStats>> BowlingStatsList { get; set; }
        public List<WeeklyBowlerStats> WeeklyBowlingStatsList { get; set; }
        public int MaxSeriesLeft { get; set; }
    }
}