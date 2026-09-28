using System;
using System.Threading;
using System.Threading.Tasks;
using App.Core;
using App.Data.Dto;
using App.Data.Exceptions;
using App.Data.Repositories;
using App.Domain;
using App.Tests.EditMode.Fakes;
using App.Tests.EditMode.Fixtures;
using Newtonsoft.Json;
using NUnit.Framework;

namespace App.Tests.EditMode.Data
{
    public class PixabaySearchRepositoryTests
    {
        private static FakeImageSearchDataSource FakeWithResponse(string json)
        {
            return new FakeImageSearchDataSource
            {
                ResponseToReturn = JsonConvert.DeserializeObject<PixabaySearchResponseDto>(json)
            };
        }

        [Test]
        public async Task Search_success_returns_mapped_items()
        {
            var fake = FakeWithResponse(PixabayJsonFixtures.TwoHits);
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            Assert.IsInstanceOf<Result<PagedResult<ImageItem>, NetworkError>.Success>(result);
            var success = (Result<PagedResult<ImageItem>, NetworkError>.Success)result;
            Assert.AreEqual(2, success.data.Items.Count);
        }

        [Test]
        public async Task First_page_request_passes_page_1()
        {
            var fake = FakeWithResponse(PixabayJsonFixtures.NoHits);
            var repository = new PixabaySearchRepository(fake);

            await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            Assert.AreEqual(1, fake.CallCount);
            Assert.AreEqual(1, fake.LastPage);
        }

        [Test]
        public async Task Load_more_passes_requested_page()
        {
            var fake = FakeWithResponse(PixabayJsonFixtures.NoHits);
            var repository = new PixabaySearchRepository(fake);

            await repository.SearchAsync(new SearchQuery("cat"), 2, CancellationToken.None);

            Assert.AreEqual(2, fake.LastPage);
        }

        [Test]
        public async Task Keyword_is_passed_through_to_data_source()
        {
            var fake = FakeWithResponse(PixabayJsonFixtures.NoHits);
            var repository = new PixabaySearchRepository(fake);

            await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            Assert.AreEqual("cat", fake.LastKeyword);
        }

        [Test]
        public async Task No_results_returns_success_with_empty_list()
        {
            var fake = FakeWithResponse(PixabayJsonFixtures.NoHits);
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            var success = (Result<PagedResult<ImageItem>, NetworkError>.Success)result;
            Assert.AreEqual(0, success.data.Items.Count);
            Assert.IsFalse(success.data.HasMore);
        }

        [Test]
        public async Task Http_500_maps_to_ServerError()
        {
            var fake = new FakeImageSearchDataSource { ExceptionToThrow = new PixabayApiException(500, "server error") };
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            var error = (Result<PagedResult<ImageItem>, NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.ServerError, error.error);
        }

        [Test]
        public async Task Http_404_maps_to_NotFound()
        {
            var fake = new FakeImageSearchDataSource { ExceptionToThrow = new PixabayApiException(404, "not found") };
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            var error = (Result<PagedResult<ImageItem>, NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.NotFound, error.error);
        }

        [Test]
        public async Task Connection_failure_maps_to_NoInternet()
        {
            var fake = new FakeImageSearchDataSource { ExceptionToThrow = new NetworkUnavailableException("no connection") };
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            var error = (Result<PagedResult<ImageItem>, NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.NoInternet, error.error);
        }

        [Test]
        public async Task Timeout_maps_to_NetworkTimeout()
        {
            var fake = new FakeImageSearchDataSource { ExceptionToThrow = new NetworkTimeoutException("timed out") };
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            var error = (Result<PagedResult<ImageItem>, NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.NetworkTimeout, error.error);
        }

        [Test]
        public async Task Unknown_exception_maps_to_Unknown()
        {
            var fake = new FakeImageSearchDataSource { ExceptionToThrow = new InvalidOperationException("boom") };
            var repository = new PixabaySearchRepository(fake);

            var result = await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None);

            var error = (Result<PagedResult<ImageItem>, NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.Unknown, error.error);
        }

        [Test]
        public void Cancellation_exception_from_data_source_propagates()
        {
            var fake = new FakeImageSearchDataSource { ExceptionToThrow = new OperationCanceledException() };
            var repository = new PixabaySearchRepository(fake);

            // CatchAsync (not ThrowsAsync) because UniTask surfaces cancellation as
            // TaskCanceledException, a subtype of OperationCanceledException; the contract
            // only requires "an OperationCanceledException", not that exact runtime type.
            Assert.CatchAsync<OperationCanceledException>(async () =>
                await repository.SearchAsync(new SearchQuery("cat"), 1, CancellationToken.None));
        }

        [Test]
        public void Pre_cancelled_token_does_not_call_data_source()
        {
            var fake = new FakeImageSearchDataSource();
            var repository = new PixabaySearchRepository(fake);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.CatchAsync<OperationCanceledException>(async () =>
                await repository.SearchAsync(new SearchQuery("cat"), 1, cts.Token));

            Assert.AreEqual(0, fake.CallCount);
        }
    }
}
