using App.Composition.Mock;
using App.Data.Repositories;
using App.Domain;
using App.Presentation;
using UnityEngine;

namespace App.Composition
{
    // Composition root for the Search screen: the one place allowed to know about both
    // the concrete Data-layer implementations and the Presentation view, and wire them
    // together behind the Domain interfaces.
    public sealed class SearchScreenInstaller : MonoBehaviour
    {
        [SerializeField] private MockPixabaySearchDataSource _searchDataSource;
        [SerializeField] private MockThumbnailDataSource _thumbnailDataSource;
        [SerializeField] private SearchScreenView _view;

        public void Configure(MockPixabaySearchDataSource searchDataSource, MockThumbnailDataSource thumbnailDataSource, SearchScreenView view)
        {
            _searchDataSource = searchDataSource;
            _thumbnailDataSource = thumbnailDataSource;
            _view = view;
        }

        private void Awake()
        {
            IImageSearchRepository searchRepository = new PixabaySearchRepository(_searchDataSource);
            IThumbnailRepository thumbnailRepository = new ThumbnailRepository(_thumbnailDataSource);
            var viewModel = new SearchViewModel(searchRepository);
            _view.Initialize(viewModel, thumbnailRepository);
        }
    }
}
