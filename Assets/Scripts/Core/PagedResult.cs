using System.Collections.Generic;

namespace App.Core
{
    public sealed class PagedResult<T>
    {
        public PagedResult(IReadOnlyList<T> items, bool hasMore)
        {
            Items = items;
            HasMore = hasMore;
        }

        public IReadOnlyList<T> Items { get; }
        public bool HasMore { get; }
    }
}
