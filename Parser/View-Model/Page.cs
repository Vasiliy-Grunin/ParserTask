using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parser
{
    internal class Page : IPage
    {
        private string url;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Page(string url)
        {
            this.url = url;
        }
        /// <summary>
        /// Url for parse
        /// </summary>
        public string Url
        {
            get => url;
            private set
            {
                var match =
                    new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"
,RegexOptions.IgnoreCase);
                try
                {
                    Contract.Assert(match.IsMatch(value), "error input");
                    url = value;
                    logger.Info("Успешное создание объекта Page");
                }
                catch (Exception ex)
                {
                    logger.Warn(ex, "На вход дана не подходящая ссылка для парсинга");
                }
            }
        }

        /// <summary>
        /// All words in this url
        /// </summary>
        public List<string> Words
        {
            get
            {
                var text = ParseText();
                return GetWords(text);
            }
        }

        public List<Statistics> Statistics => GetStatistics(Words);


        /// <summary>
        /// counts  the repetition on each word in the input list
        /// </summary>
        /// <param name="words">list of words</param>
        /// <returns>new Dictionary where Key is word and Value the number of its repetitions</returns>
        public static List<Statistics> GetStatistics(List<string> words)
        {
            var statistics = new Dictionary<string, int>();
            var result = new List<Statistics>();
            foreach (var word in words)
            {
                if (statistics.ContainsKey(word))
                    statistics[word]++;
                else
                    statistics.Add(word, 1);
            }
            var sortStatistics = statistics
                .OrderBy(k => k.Key)
                .ToDictionary(x => x.Key, v => v.Value);
            foreach (var elem in sortStatistics)
                result.Add(new Statistics(elem.Key, elem.Value));
            return result;
        }

        /// <summary>
        /// the text in the list of offers format is submittedfor input.
        /// </summary>
        /// <param name="text">list content</param>
        /// <returns>list of words contained in this text</returns>
        public static List<string> GetWords(List<string> text)
        {
            var words = new List<string>();
            foreach (var line in text)
            {
                var cleanLine = line
                    .ToLowerInvariant()
                    .Split(new char[] { ',', ';', ':', '!', '?', '.' });
                foreach (var word in cleanLine)
                {
                    var clearWord = word
                    .Trim(new char[] { ',', ';', ':', '!', '?', '\"', '\'',
                        '(', ')', '}', '{', '[', ']', '\t', '\n', '\\' });
                    var setWords = clearWord
                        .Split(' ');
                    BuildWord(words, setWords);
                }
            }
            return words;
        }

        /// <summary>
        /// cheak the string for the url format
        /// </summary>
        /// <returns>if string url true
        /// else false</returns>
        public static bool IsUrl(string url)
            => new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)")
            .IsMatch(url);

        private static void BuildWord(List<string> words, string[] setWords)
        {
            foreach (var setSymbol in setWords)
            {
                string word = "";
                for (int i = 0; i < setSymbol.Length; i++)
                {
                    if (char.IsLetter(setSymbol[i]))
                        word += setSymbol[i];
                    else if (word.Length > 0
                        && '-'.Equals(setSymbol[i])
                        && i + 1 < setSymbol.Length
                        && char.IsLetter(setSymbol[i + 1]))
                        word += setSymbol[i];
                }
                if (!string.IsNullOrWhiteSpace(word))
                    words.Add(word.ToLowerInvariant());
            }
        }

        private List<string> ParseText()
        {
            var htmlWeb = new HtmlWeb();
            try
            {
                htmlWeb.OverrideEncoding = System.Text.Encoding.UTF8;
                var doc = htmlWeb.Load(Url);
                var textCollection = doc.DocumentNode.SelectNodes("//body");
                var result = new List<string>();
                foreach (var node in textCollection)
                    foreach (var lavel in node.ChildNodes)
                        GetContent(lavel, result);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ошибка подключения к странице");
                return new List<string>();
            }
        }

        private void GetContent(HtmlNode htmlNodes, List<string> words)
        {
            if (htmlNodes.ChildNodes.Count == 0)
            {
                if (htmlNodes.InnerText.IsNormalized()
                    && !string.IsNullOrWhiteSpace(htmlNodes.InnerText)
                    && !htmlNodes.XPath.Contains("script"))
                    words.Add(htmlNodes.InnerText.Normalize());
                return;
            }

            foreach (var childNode in htmlNodes.ChildNodes)
                GetContent(childNode, words);
        }
    }
}
