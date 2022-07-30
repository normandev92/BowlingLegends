using System.Data.Entity;

namespace BowlingLegends.Models
{
    public partial class BowlingLegendsContext : DbContext
    {
        public BowlingLegendsContext()
            : base("name=BowlingLegendsContext")
        {
        }

        public virtual DbSet<Bowler> Bowlers { get; set; }
        public virtual DbSet<Round> Rounds { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<Series> Series { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bowler>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Bowler>()
                .Property(e => e.NickName)
                .IsUnicode(false);

            modelBuilder.Entity<Bowler>()
                .Property(e => e.BowlingArm)
                .IsUnicode(false);

            modelBuilder.Entity<Bowler>()
                .HasMany(e => e.Scores)
                .WithRequired(e => e.Bowler)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Round>()
                .HasMany(e => e.Scores)
                .WithRequired(e => e.Round)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Season>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<Season>()
                .HasMany(e => e.Series)
                .WithRequired(e => e.Season)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Series>()
                .HasMany(e => e.Rounds)
                .WithRequired(e => e.Series)
                .WillCascadeOnDelete(false);
        }
    }
}
