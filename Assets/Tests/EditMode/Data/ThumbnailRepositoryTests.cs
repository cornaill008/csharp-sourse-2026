using System;
using System.Threading;
using System.Threading.Tasks;
using App.Core;
using App.Data.Exceptions;
using App.Data.Repositories;
using App.Tests.EditMode.Fakes;
using NUnit.Framework;

namespace App.Tests.EditMode.Data
{
    public class ThumbnailRepositoryTests
    {
        [Test]
        public async Task Download_success_returns_bytes()
        {
            var bytes = new byte[] { 1, 2, 3 };
            var fake = new FakeThumbnailDataSource { BytesToReturn = bytes };
            var repository = new ThumbnailRepository(fake);

            var result = await repository.GetAsync("https://example.com/x.jpg", CancellationToken.None);

            var success = (Result<byte[], NetworkError>.Success)result;
            Assert.AreEqual(bytes, success.data);
        }

        [Test]
        public async Task Http_500_maps_to_ServerError()
        {
            var fake = new FakeThumbnailDataSource { ExceptionToThrow = new PixabayApiException(500, "server error") };
            var repository = new ThumbnailRepository(fake);

            var result = await repository.GetAsync("https://example.com/x.jpg", CancellationToken.None);

            var error = (Result<byte[], NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.ServerError, error.error);
        }

        [Test]
        public async Task Connection_failure_maps_to_NoInternet()
        {
            var fake = new FakeThumbnailDataSource { ExceptionToThrow = new NetworkUnavailableException("no connection") };
            var repository = new ThumbnailRepository(fake);

            var result = await repository.GetAsync("https://example.com/x.jpg", CancellationToken.None);

            var error = (Result<byte[], NetworkError>.Error)result;
            Assert.AreEqual(NetworkError.NoInternet, error.error);
        }

        [Test]
        public void Cancellation_exception_from_data_source_propagates()
        {
            var fake = new FakeThumbnailDataSource { ExceptionToThrow = new OperationCanceledException() };
            var repository = new ThumbnailRepository(fake);

            // CatchAsync (not ThrowsAsync) because UniTask surfaces cancellation as
            // TaskCanceledException, a subtype of OperationCanceledException; the contract
            // only requires "an OperationCanceledException", not that exact runtime type.
            Assert.CatchAsync<OperationCanceledException>(async () =>
                await repository.GetAsync("https://example.com/x.jpg", CancellationToken.None));
        }

        [Test]
        public void Pre_cancelled_token_does_not_call_data_source()
        {
            var fake = new FakeThumbnailDataSource();
            var repository = new ThumbnailRepository(fake);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.CatchAsync<OperationCanceledException>(async () =>
                await repository.GetAsync("https://example.com/x.jpg", cts.Token));

            Assert.AreEqual(0, fake.CallCount);
        }
    }
}
