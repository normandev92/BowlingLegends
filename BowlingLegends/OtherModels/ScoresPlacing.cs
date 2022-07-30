using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BowlingLegends.Models;

namespace BowlingLegends.OtherModels
{
    public class ScoresPlacing
    {
        public int Id { get; set; }
        public int? Score { get; set; }
        public int? Placing { get; set; }
    }
}