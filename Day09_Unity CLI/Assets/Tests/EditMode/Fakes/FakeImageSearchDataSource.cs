using System;
using System.Threading;
using App.Data.DataSources;
using App.Data.Dto;
using Cysharp.Threading.Tasks;

namespace App.Tests.EditMode.Fakes
{
    public sealed class FakeImageSearchDataSource : IPixabaySearchDataSource
    {
        public PixabaySearchResponseDto ResponseToReturn { get; set; }
        public Exception ExceptionToThrow { get; set; }

        public int CallCount { get; private set; }
        public string LastKeyword { get; private set; }
        public int LastPage { get; private set; }

        public UniTask<PixabaySearchResponseDto> SearchAsync(string keyword, int page, CancellationToken cancellationToken)
        {
            CallCount++;
            LastKeyword = keyword;
            LastPage = page;

            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            return UniTask.FromResult(ResponseToReturn);
        }
    }
}
