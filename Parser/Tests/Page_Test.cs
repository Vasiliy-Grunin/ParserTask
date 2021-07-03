using System.Linq;
using NUnit.Framework;

namespace Parser
{
    [TestFixture]
    public class Page_Test
    {
        [TestCase("url",false)]
        [TestCase("2134356789", false)]
        [TestCase(@"https://habr.com/ru/post/560318/",true)]
        [TestCase(@"https://www.simbirsoft.com/", true)]
        [TestCase(@"12@#$^&($hfke[]",false)]

        [Test]
        public void TestUrl(string url, bool expected)
        {
            var actual = Page.IsUrl(url);
            Assert.AreEqual(expected, actual, "error in " + url);
        }

        [TestCase(new string[] { "a", "a", "b" }, new string[] {"a","b"}, new int[]{ 2, 1 })]
        [TestCase(new string[] { }, new string[] { }, new int[] { })]

        [Test]
        public void TestStatistics(string[] inputArray, string[] key, int[] value)
        {
            var actual = Page.GetStatistics(inputArray.ToList());
            for (int i = 0; i < key.Length; i++)
            { 
                Assert.AreEqual(key[i], actual[i].Word, "error key");
                Assert.AreEqual(value[i], actual[i].Count, "error value");
            }
        }

        [TestCase(new string[]{ " ", " " }, new string[] { })]
        [TestCase(new string[] { "((((a^%^","rt","!@#$%^yt"},new string[] { "a","rt","yt"})]
        [TestCase(new string[] { },new string[] { })]
        [TestCase(new string[] {"!@#$%^&.,*()_aa+=/\\","aa" }, new string[] { "aa","aa"})]
        [TestCase(new string[] { "\nn\t","\ryt"}, new string[] { "n","yt"})]
        [TestCase(new string[] {"\\\\\\\\\\","/////////" },new string[] { })]
        [TestCase(new string[] { "8435 %$ yts65","8n9"}, new string[] { "yts","n"})]
        [TestCase(new string[] { "---t-","-----"}, new string[] { "t"})]
        [TestCase(new string[] {"Ac-09","90url" },new string[] { "ac","url"})]

        [Test]
        public void TestGetWords(string[] text, string[] expected)
        {
            var actual = Page.GetWords(text.ToList());
            Assert.AreEqual(expected, actual, "error");
        }
    }
}
