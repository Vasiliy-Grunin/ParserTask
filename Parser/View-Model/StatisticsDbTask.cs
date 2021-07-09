using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    internal class StatisticsDbTask
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// saves statistics an the page
        /// </summary>
        public static void SaveStatistics(Page page)
        {
            try
            {
                using (var db = new StatisticsDbContext())
                { 
                    foreach (var elem in page.Statistics)
                        db.OldStatistics.Add(new Statistics(elem.Word, elem.Count, page.Url));
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ошибка добавление записей в бд");
            }
        }

        /// <summary>
        /// makes request to the database to exclude all fields
        /// </summary>
        /// <returns> all saved statistics</returns>
        public static List<Statistics> DownloadHistory()
        {
            try
            {
                using (var db = new StatisticsDbContext())
                { 
                    var history = db.OldStatistics.Select(x => x).OrderBy(x => x.Word).ToList();
                    return history;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ошибка подключения к бд");
                throw new ArgumentException();
            }
        }
        

        /// <summary>
        /// count a repetion every word in database
        /// </summary>
        /// <returns>dictionary where key is word and value count</returns>
        public static Dictionary<string,int> GetStatisticsDb()
        {
            using (var db = new StatisticsDbContext())
            {
                var statistics = db.OldStatistics;
                var result = new Dictionary<string, int>();
                foreach (var elem in statistics)
                {
                    if (result.ContainsKey(elem.Word))
                        result[elem.Word]++;
                    else
                        result.Add(elem.Word, elem.Count);
                }
                return result;
            }
        }

        /// <summary>
        /// delete statistics for input url
        /// </summary>
        /// <param name="url">url for clear database</param>
        public static void RemoveStatistics(string url)
        {
            using (var db = new StatisticsDbContext())
            {
                var deletStatistics = db.OldStatistics.Where(x => x.Url.Equals(url)).ToList();
                foreach (var elem in deletStatistics)
                    db.OldStatistics.Remove(elem);
                db.SaveChanges();
            }
            logger.Info("удалена статистика с url: " + url);
        }
    }
}
