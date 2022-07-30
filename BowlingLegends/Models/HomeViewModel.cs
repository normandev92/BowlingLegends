using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BowlingLegends.OtherModels;

namespace BowlingLegends.Models
{
    public class HomeViewModel
    {
        public List<BowlingStats> BowlingStats { get; set; }
        public int PotMoney { get; set; }
        public DateTime FinalDate { get; set; }
        public int MaxSeriesLeft { get; set; }
        public int ExpectedPotMoney { get; set; }
        public int MaxPoints { get; set; }
    }
}