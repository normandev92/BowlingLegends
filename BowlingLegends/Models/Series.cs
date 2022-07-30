using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace BowlingLegends.Models
{
    public class Series
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Series()
        {
            Rounds = new HashSet<Round>();
        }

        public int SeriesID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int SeasonID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Round> Rounds { get; set; }

        public virtual Season Season { get; set; }

    }
}
