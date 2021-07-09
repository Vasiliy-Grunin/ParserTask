using System.Data.Entity;

namespace Parser
{
    internal class StatisticsDbContext: DbContext
    {
        public StatisticsDbContext() //: base("Parser.StatisticsDbContext")
        { 
        }

        public virtual DbSet<Statistics> OldStatistics { get; set; } 

    }
}
