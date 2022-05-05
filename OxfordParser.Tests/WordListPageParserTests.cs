using AngleSharp.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxfordParser.Tests
{
    [TestClass]
    public class WordListPageParserTests
    {
        private readonly WordListPageParser wordListPageParser = new WordListPageParser();

        [TestMethod]
        public void AllModelFieldsMustBeFilled()
        {
            var html = File.ReadAllText("3000-5000.html");
            var enumerable = wordListPageParser.GetWordListItems(html);
            var totalItems = 3;
            var current = 0;
            foreach (var item in enumerable)
            {
                if (current == totalItems)
                    return;
                Assert.IsNotNull(item);
                Assert.IsNotNull(item.DetailsLink);
                Assert.IsNotNull(item.UKSoundLink);
                Assert.IsNotNull(item.AmericanSoundLink);
                Assert.IsNotNull(item.WordText);
                Assert.IsNotNull(item.Type);
                Assert.IsTrue(item.Category3000.HasValue || item.Category5000.HasValue);
                current++;
            }
        }
    }
}
