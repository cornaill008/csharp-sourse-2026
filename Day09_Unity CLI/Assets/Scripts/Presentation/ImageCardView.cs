using System;
using System.Threading;
using App.Core;
using App.Domain;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation
{
    public sealed class ImageCardView : MonoBehaviour
    {
        private static readonly Color PlaceholderColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        [SerializeField] private Image _thumbnail;
        [SerializeField] private TextMeshProUGUI _tagsText;
        [SerializeField] private TextMeshProUGUI _authorText;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            if (_thumbnail == null)
            {
                _thumbnail = transform.Find("Thumbnail").GetComponent<Image>();
            }
            if (_tagsText == null)
            {
                _tagsText = transform.Find("TagsText").GetComponent<TextMeshProUGUI>();
            }
            if (_authorText == null)
            {
                _authorText = transform.Find("AuthorText").GetComponent<TextMeshProUGUI>();
            }
        }

        public void Bind(ImageItem item, IThumbnailRepository thumbnailRepository)
        {
            _tagsText.text = string.Join(", ", item.Tags);
            _authorText.text = item.AuthorName;
            _thumbnail.sprite = null;
            _thumbnail.color = PlaceholderColor;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            LoadThumbnailAsync(item.ThumbnailUrl, thumbnailRepository, _cts.Token).Forget();
        }

        private async UniTaskVoid LoadThumbnailAsync(string url, IThumbnailRepository repository, CancellationToken token)
        {
            Result<byte[], NetworkError> result;
            try
            {
                result = await repository.GetAsync(url, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (token.IsCancellationRequested || this == null)
            {
                return;
            }

            if (result is Result<byte[], NetworkError>.Success success)
            {
                var texture = new Texture2D(2, 2);
                if (texture.LoadImage(success.data))
                {
                    var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    _thumbnail.sprite = sprite;
                    _thumbnail.color = Color.white;
                    return;
                }
            }

            // Failure or undecodable bytes: keep the neutral placeholder color already set in Bind().
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
        }
    }
}
