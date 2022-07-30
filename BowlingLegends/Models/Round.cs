using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BowlingLegends.Models
{
    [Table("Round")]
    public partial class Round
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Round()
        {
            Scores = new HashSet<Score>();
        }

        public int RoundID { get; set; }

        public int SeriesID { get; set; }

        public virtual Series Series { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Score> Scores { get; set; }
    }
}
