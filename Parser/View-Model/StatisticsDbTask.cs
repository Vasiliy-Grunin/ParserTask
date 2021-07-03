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
            var db = new StatisticsDbContext();
            try
            {
                foreach (var elem in page.Statistics)
                    db.OldStatistics.Add(new Statistics(elem.Word, elem.Count, page.Url));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ошибка добавление записей в бд");
                throw new ArgumentException();
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ошибка сохранения записей в бд");
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
                var db = new StatisticsDbContext();
                var history = db.OldStatistics.Select(x => x).OrderBy(x => x.Word).ToList();
                return history;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ошибка подключения к бд");
                throw new ArgumentException();
            }
        }
    }
}
