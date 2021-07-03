using System;
using System.ComponentModel.DataAnnotations;

namespace Parser
{
    internal class Statistics : IStatistics
    {
        public Statistics() { }

        public Statistics(string url)
        {
            Url = url;
        }

        public Statistics(string word, int count)
        {
            Word = word;
            Count = count;
        }

        public Statistics(string word, int count, string url)
        {
            Word = word;
            Count = count;
            Url = url;
        }

        [Key]
        public int Id { get; set; }
        public string Word { get; set; }
        public int Count { get; set; }
        public string Url { get; set; }

        public override string ToString() => string.Format("{0} - {1} = {2}", Url, Word, Count);
    }
}
