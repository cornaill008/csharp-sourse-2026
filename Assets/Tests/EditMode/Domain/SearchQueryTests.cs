using System;
using App.Domain;
using NUnit.Framework;

namespace App.Tests.EditMode.Domain
{
    public class SearchQueryTests
    {
        [Test]
        public void Empty_keyword_throws()
        {
            Assert.Throws<ArgumentException>(() => new SearchQuery(""));
        }

        [Test]
        public void Whitespace_only_keyword_throws()
        {
            Assert.Throws<ArgumentException>(() => new SearchQuery("   "));
        }

        [Test]
        public void Keyword_is_trimmed()
        {
            var query = new SearchQuery("  cat  ");

            Assert.AreEqual("cat", query.Keyword);
        }
    }
}
