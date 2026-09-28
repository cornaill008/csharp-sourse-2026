using System;
using System.Threading;
using App.Data.Dto;
using App.Data.Exceptions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace App.Data.DataSources
{
    public sealed class UnityWebRequestSearchDataSource : IPixabaySearchDataSource
    {
        private const int TimeoutSeconds = 10;

        private readonly string _apiKey;

        public UnityWebRequestSearchDataSource(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("apiKey must not be null or empty.", nameof(apiKey));
            }

            _apiKey = apiKey;
        }

        public async UniTask<PixabaySearchResponseDto> SearchAsync(string keyword, int page, CancellationToken cancellationToken)
        {
            var url = PixabayRequestUrlBuilder.Build(_apiKey, keyword, page);

            using var request = UnityWebRequest.Get(url);
            request.timeout = TimeoutSeconds;

            await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                if (request.error != null && request.error.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new NetworkTimeoutException(request.error);
                }

                throw new NetworkUnavailableException(request.error);
            }

            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new PixabayApiException((int)request.responseCode, request.error);
            }

            return JsonConvert.DeserializeObject<PixabaySearchResponseDto>(request.downloadHandler.text);
        }
    }
}
