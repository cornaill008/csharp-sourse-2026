using System.Threading;
using App.Core;
using Cysharp.Threading.Tasks;

namespace App.Domain
{
    public interface IThumbnailRepository
    {
        // Cancellation propagates as OperationCanceledException; it is never wrapped in Result.
        UniTask<Result<byte[], NetworkError>> GetAsync(string url, CancellationToken cancellationToken);
    }
}
