using System.Collections.Generic;

namespace App.Domain
{
    public sealed class ImageItem
    {
        public ImageItem(string id, string thumbnailUrl, IReadOnlyList<string> tags, string authorName)
        {
            Id = id;
            ThumbnailUrl = thumbnailUrl;
            Tags = tags;
            AuthorName = authorName;
        }

        public string Id { get; }
        public string ThumbnailUrl { get; }
        public IReadOnlyList<string> Tags { get; }
        public string AuthorName { get; }
    }
}
