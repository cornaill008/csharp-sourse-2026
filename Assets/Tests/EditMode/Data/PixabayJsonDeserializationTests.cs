using App.Data.Dto;
using App.Tests.EditMode.Fixtures;
using Newtonsoft.Json;
using NUnit.Framework;

namespace App.Tests.EditMode.Data
{
    public class PixabayJsonDeserializationTests
    {
        [Test]
        public void Deserializes_two_hits()
        {
            var dto = JsonConvert.DeserializeObject<PixabaySearchResponseDto>(PixabayJsonFixtures.TwoHits);

            Assert.AreEqual(2, dto.totalHits);
            Assert.AreEqual(2, dto.hits.Count);
            Assert.AreEqual("alice", dto.hits[0].user);
        }

        [Test]
        public void Deserializes_no_hits()
        {
            var dto = JsonConvert.DeserializeObject<PixabaySearchResponseDto>(PixabayJsonFixtures.NoHits);

            Assert.AreEqual(0, dto.totalHits);
            Assert.AreEqual(0, dto.hits.Count);
        }
    }
}
