using App.Data.Dto;
using App.Data.Mapping;
using NUnit.Framework;

namespace App.Tests.EditMode.Data
{
    public class ImageItemMapperTests
    {
        [Test]
        public void Maps_basic_fields()
        {
            var dto = new PixabayHitDto { id = 1, tags = "cat", previewURL = "p", webformatURL = "w", user = "alice" };

            var item = ImageItemMapper.ToModel(dto);

            Assert.AreEqual("1", item.Id);
            Assert.AreEqual("w", item.ThumbnailUrl);
            Assert.AreEqual("alice", item.AuthorName);
        }

        [Test]
        public void Splits_tags_by_comma()
        {
            var tags = ImageItemMapper.ParseTags("cat, animal, pet");

            CollectionAssert.AreEqual(new[] { "cat", "animal", "pet" }, tags);
        }

        [Test]
        public void Trims_whitespace_around_tags()
        {
            var tags = ImageItemMapper.ParseTags(" cat , animal ");

            CollectionAssert.AreEqual(new[] { "cat", "animal" }, tags);
        }

        [Test]
        public void Removes_empty_entries_between_commas()
        {
            var tags = ImageItemMapper.ParseTags("cat,, animal");

            CollectionAssert.AreEqual(new[] { "cat", "animal" }, tags);
        }

        [Test]
        public void Removes_whitespace_only_entries()
        {
            var tags = ImageItemMapper.ParseTags("cat,   , animal");

            CollectionAssert.AreEqual(new[] { "cat", "animal" }, tags);
        }

        [Test]
        public void Empty_string_tags_returns_empty_list()
        {
            var tags = ImageItemMapper.ParseTags("");

            Assert.AreEqual(0, tags.Count);
        }

        [Test]
        public void Null_tags_returns_empty_list()
        {
            var tags = ImageItemMapper.ParseTags(null);

            Assert.AreEqual(0, tags.Count);
        }
    }
}
