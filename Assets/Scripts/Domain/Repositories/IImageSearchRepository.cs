using System.Threading;
using App.Core;
using Cysharp.Threading.Tasks;

namespace App.Domain
{
    public interface IImageSearchRepository
    {
        // Cancellation propagates as OperationCanceledException; it is never wrapped in Result.
        UniTask<Result<PagedResult<ImageItem>, NetworkError>> SearchAsync(SearchQuery query, int page, CancellationToken cancellationToken);
    }
}
