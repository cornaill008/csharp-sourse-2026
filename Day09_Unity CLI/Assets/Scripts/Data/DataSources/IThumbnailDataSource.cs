using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Data.DataSources
{
    // Same exception contract as IPixabaySearchDataSource.
    public interface IThumbnailDataSource
    {
        UniTask<byte[]> DownloadAsync(string url, CancellationToken cancellationToken);
    }
}
