using System;

namespace App.Domain
{
    public sealed class SearchQuery
    {
        public SearchQuery(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("keyword must not be empty or whitespace.", nameof(keyword));
            }

            Keyword = keyword.Trim();
        }

        public string Keyword { get; }
    }
}
