using System.Data.Entity;

namespace Parser
{
    internal class StatisticsDbContext: DbContext
    {
        public StatisticsDbContext()
        { 
        }

        public virtual DbSet<Statistics> OldStatistics { get; set; } 

    }
}
