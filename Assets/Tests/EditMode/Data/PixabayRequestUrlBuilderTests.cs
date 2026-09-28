using System;
using App.Data.DataSources;
using NUnit.Framework;

namespace App.Tests.EditMode.Data
{
    public class PixabayRequestUrlBuilderTests
    {
        [Test]
        public void Includes_key_keyword_and_page()
        {
            var url = PixabayRequestUrlBuilder.Build("KEY123", "cat", 2);

            StringAssert.Contains("key=KEY123", url);
            StringAssert.Contains("q=cat", url);
            StringAssert.Contains("page=2", url);
        }

        [Test]
        public void Throws_when_api_key_missing()
        {
            Assert.Throws<ArgumentException>(() => PixabayRequestUrlBuilder.Build("", "cat", 1));
        }
    }
}
