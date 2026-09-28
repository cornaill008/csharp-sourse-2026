using System.Threading;
using App.Data.Dto;
using Cysharp.Threading.Tasks;

namespace App.Data.DataSources
{
    // Throws NetworkTimeoutException / NetworkUnavailableException / PixabayApiException on failure.
    // OperationCanceledException (cancellationToken cancelled) propagates as-is.
    public interface IPixabaySearchDataSource
    {
        UniTask<PixabaySearchResponseDto> SearchAsync(string keyword, int page, CancellationToken cancellationToken);
    }
}
