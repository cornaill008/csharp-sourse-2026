using System.Threading;
using App.Data.DataSources;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Composition.Mock
{
    // Generates a small solid-color PNG per URL instead of downloading a real image.
    public sealed class MockThumbnailDataSource : MonoBehaviour, IThumbnailDataSource
    {
        [SerializeField] private int _artificialDelayMs = 300;

        public async UniTask<byte[]> DownloadAsync(string url, CancellationToken cancellationToken)
        {
            await UniTask.Delay(_artificialDelayMs, cancellationToken: cancellationToken);

            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            var color = ColorFromUrl(url);
            var pixels = new Color32[64 * 64];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels32(pixels);
            texture.Apply();

            var bytes = texture.EncodeToPNG();
            Destroy(texture);
            return bytes;
        }

        private static Color32 ColorFromUrl(string url)
        {
            var hash = url != null ? url.GetHashCode() : 0;
            var hue = System.Math.Abs(hash % 360) / 360f;
            return Color.HSVToRGB(hue, 0.35f, 0.9f);
        }
    }
}
