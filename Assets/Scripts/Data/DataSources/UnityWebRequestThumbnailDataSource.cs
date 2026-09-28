using System;
using System.Threading;
using App.Data.Exceptions;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace App.Data.DataSources
{
    public sealed class UnityWebRequestThumbnailDataSource : IThumbnailDataSource
    {
        private const int TimeoutSeconds = 10;

        public async UniTask<byte[]> DownloadAsync(string url, CancellationToken cancellationToken)
        {
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

            return request.downloadHandler.data;
        }
    }
}
