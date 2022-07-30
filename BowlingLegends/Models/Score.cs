using System.ComponentModel.DataAnnotations.Schema;

namespace BowlingLegends.Models
{
    [Table("Score")]
    public partial class Score
    {
        public int ScoreID { get; set; }

        public int BowlerID { get; set; }

        [Column("Score")]
        public int Score1 { get; set; }

        public int RoundID { get; set; }

        public virtual Bowler Bowler { get; set; }

        public virtual Round Round { get; set; }
    }
}
