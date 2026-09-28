using System;
using System.Threading;
using App.Core;
using App.Data.DataSources;
using App.Data.Exceptions;
using App.Domain;
using Cysharp.Threading.Tasks;

namespace App.Data.Repositories
{
    public sealed class ThumbnailRepository : IThumbnailRepository
    {
        private readonly IThumbnailDataSource _dataSource;

        public ThumbnailRepository(IThumbnailDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        public async UniTask<Result<byte[], NetworkError>> GetAsync(string url, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var bytes = await _dataSource.DownloadAsync(url, cancellationToken);
                return new Result<byte[], NetworkError>.Success(bytes);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NetworkTimeoutException)
            {
                return new Result<byte[], NetworkError>.Error(NetworkError.NetworkTimeout);
            }
            catch (NetworkUnavailableException)
            {
                return new Result<byte[], NetworkError>.Error(NetworkError.NoInternet);
            }
            catch (PixabayApiException ex) when (ex.StatusCode == 404)
            {
                return new Result<byte[], NetworkError>.Error(NetworkError.NotFound);
            }
            catch (PixabayApiException)
            {
                return new Result<byte[], NetworkError>.Error(NetworkError.ServerError);
            }
            catch (Exception)
            {
                return new Result<byte[], NetworkError>.Error(NetworkError.Unknown);
            }
        }
    }
}
