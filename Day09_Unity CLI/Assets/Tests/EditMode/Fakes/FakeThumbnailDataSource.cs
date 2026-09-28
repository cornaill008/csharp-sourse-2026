using System;
using System.Threading;
using App.Data.DataSources;
using Cysharp.Threading.Tasks;

namespace App.Tests.EditMode.Fakes
{
    public sealed class FakeThumbnailDataSource : IThumbnailDataSource
    {
        public byte[] BytesToReturn { get; set; }
        public Exception ExceptionToThrow { get; set; }

        public int CallCount { get; private set; }
        public string LastUrl { get; private set; }

        public UniTask<byte[]> DownloadAsync(string url, CancellationToken cancellationToken)
        {
            CallCount++;
            LastUrl = url;

            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            return UniTask.FromResult(BytesToReturn);
        }
    }
}
