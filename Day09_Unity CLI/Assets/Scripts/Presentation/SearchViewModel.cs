using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Core;
using App.Domain;
using Cysharp.Threading.Tasks;

namespace App.Presentation
{
    public sealed class SearchViewModel
    {
        private readonly IImageSearchRepository _repository;
        private CancellationTokenSource _cts;
        private SearchQuery _currentQuery;
        private int _currentPage;

        public SearchViewModel(IImageSearchRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Items = Array.Empty<ImageItem>();
        }

        public event Action Changed;

        public SearchScreenState State { get; private set; } = SearchScreenState.Idle;
        public IReadOnlyList<ImageItem> Items { get; private set; }
        public bool HasMore { get; private set; }
        public bool LastPageWasFirstPage { get; private set; }
        public string ErrorMessage { get; private set; }

        public void Search(string keyword)
        {
            SearchQuery query;
            try
            {
                query = new SearchQuery(keyword);
            }
            catch (ArgumentException)
            {
                return;
            }

            _currentQuery = query;
            _currentPage = 1;
            StartRequest(SearchScreenState.Loading, query, 1);
        }

        public void LoadMore()
        {
            if (State != SearchScreenState.Success || !HasMore || _currentQuery == null)
            {
                return;
            }

            StartRequest(SearchScreenState.LoadingMore, _currentQuery, _currentPage + 1);
        }

        private void StartRequest(SearchScreenState pendingState, SearchQuery query, int page)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            SetState(pendingState);
            RunSearchAsync(query, page, _cts.Token).Forget();
        }

        private async UniTaskVoid RunSearchAsync(SearchQuery query, int page, CancellationToken token)
        {
            Result<PagedResult<ImageItem>, NetworkError> result;
            try
            {
                result = await _repository.SearchAsync(query, page, token);
            }
            catch (OperationCanceledException)
            {
                // Superseded by a newer search/load-more call; nothing to do.
                return;
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            if (result is Result<PagedResult<ImageItem>, NetworkError>.Success success)
            {
                _currentPage = page;
                LastPageWasFirstPage = page == 1;
                Items = page == 1
                    ? success.data.Items
                    : Items.Concat(success.data.Items).ToList();
                HasMore = success.data.HasMore;
                SetState(Items.Count == 0 ? SearchScreenState.Empty : SearchScreenState.Success);
            }
            else if (result is Result<PagedResult<ImageItem>, NetworkError>.Error error)
            {
                ErrorMessage = MapErrorMessage(error.error);
                SetState(SearchScreenState.Error);
            }
        }

        private static string MapErrorMessage(NetworkError error)
        {
            switch (error)
            {
                case NetworkError.NetworkTimeout:
                    return "요청 시간이 초과됐어요. 다시 시도해주세요.";
                case NetworkError.NoInternet:
                    return "인터넷 연결을 확인해주세요.";
                case NetworkError.NotFound:
                    return "요청한 정보를 찾을 수 없어요.";
                case NetworkError.ServerError:
                    return "서버에 일시적인 문제가 발생했어요. 잠시 후 다시 시도해주세요.";
                default:
                    return "알 수 없는 오류가 발생했어요.";
            }
        }

        private void SetState(SearchScreenState state)
        {
            State = state;
            Changed?.Invoke();
        }
    }
}
