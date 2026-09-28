using System;
using System.Linq;
using System.Threading;
using App.Core;
using App.Data.DataSources;
using App.Data.Exceptions;
using App.Data.Mapping;
using App.Domain;
using Cysharp.Threading.Tasks;

namespace App.Data.Repositories
{
    public sealed class PixabaySearchRepository : IImageSearchRepository
    {
        private readonly IPixabaySearchDataSource _dataSource;

        public PixabaySearchRepository(IPixabaySearchDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        public async UniTask<Result<PagedResult<ImageItem>, NetworkError>> SearchAsync(SearchQuery query, int page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var dto = await _dataSource.SearchAsync(query.Keyword, page, cancellationToken);

                var items = dto.hits.Select(ImageItemMapper.ToModel).ToList();
                var hasMore = page * PixabayRequestUrlBuilder.PerPage < dto.totalHits;

                return new Result<PagedResult<ImageItem>, NetworkError>.Success(new PagedResult<ImageItem>(items, hasMore));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NetworkTimeoutException)
            {
                return new Result<PagedResult<ImageItem>, NetworkError>.Error(NetworkError.NetworkTimeout);
            }
            catch (NetworkUnavailableException)
            {
                return new Result<PagedResult<ImageItem>, NetworkError>.Error(NetworkError.NoInternet);
            }
            catch (PixabayApiException ex) when (ex.StatusCode == 404)
            {
                return new Result<PagedResult<ImageItem>, NetworkError>.Error(NetworkError.NotFound);
            }
            catch (PixabayApiException)
            {
                return new Result<PagedResult<ImageItem>, NetworkError>.Error(NetworkError.ServerError);
            }
            catch (Exception)
            {
                return new Result<PagedResult<ImageItem>, NetworkError>.Error(NetworkError.Unknown);
            }
        }
    }
}
