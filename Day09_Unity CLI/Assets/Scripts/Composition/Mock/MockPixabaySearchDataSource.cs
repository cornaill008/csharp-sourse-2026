using System.Collections.Generic;
using System.Threading;
using App.Data.DataSources;
using App.Data.Dto;
using App.Data.Exceptions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Composition.Mock
{
    // Stands in for the real Pixabay HTTP call so the app can run end to end before the
    // real API is wired up. Not a test double (see App.Tests.EditMode.Fakes for those) —
    // this is a development/demo stand-in meant to run inside the actual app.
    public sealed class MockPixabaySearchDataSource : MonoBehaviour, IPixabaySearchDataSource
    {
        private static readonly string[] Authors = { "alice", "bob", "carol", "dave", "erin", "frank" };

        private static readonly string[][] TagSets =
        {
            new[] { "cat", "animal", "pet" },
            new[] { "dog", "animal" },
            new[] { "mountain", "nature", "sky" },
            new[] { "city", "night" },
            new[] { "food", "coffee" },
            new[] { "ocean", "beach", "summer" },
        };

        [SerializeField] private MockSearchScenario _scenario = MockSearchScenario.Success;
        [SerializeField] private int _artificialDelayMs = 600;

        public async UniTask<PixabaySearchResponseDto> SearchAsync(string keyword, int page, CancellationToken cancellationToken)
        {
            await UniTask.Delay(_artificialDelayMs, cancellationToken: cancellationToken);

            switch (_scenario)
            {
                case MockSearchScenario.EmptyResult:
                    return new PixabaySearchResponseDto { totalHits = 0, total = 0, hits = new List<PixabayHitDto>() };

                case MockSearchScenario.ConnectionFailure:
                    throw new NetworkUnavailableException("mock: connection failure");

                case MockSearchScenario.ServerError:
                    throw new PixabayApiException(500, "mock: server error");

                default:
                    return BuildSuccessResponse(keyword, page);
            }
        }

        private static PixabaySearchResponseDto BuildSuccessResponse(string keyword, int page)
        {
            var hits = new List<PixabayHitDto>();
            for (var i = 0; i < Authors.Length; i++)
            {
                var id = page * 100 + i;
                var tags = string.IsNullOrWhiteSpace(keyword)
                    ? string.Join(", ", TagSets[i])
                    : keyword + ", " + string.Join(", ", TagSets[i]);

                hits.Add(new PixabayHitDto
                {
                    id = id,
                    tags = tags,
                    previewURL = "mock://preview/" + id,
                    webformatURL = "mock://thumb/" + id,
                    user = Authors[i] + " (p" + page + ")",
                    likes = 0,
                    views = 0,
                });
            }

            // totalHits > PerPage(20) * page for a couple of pages so "load more" has something to do.
            return new PixabaySearchResponseDto { totalHits = 45, total = 45, hits = hits };
        }
    }
}
