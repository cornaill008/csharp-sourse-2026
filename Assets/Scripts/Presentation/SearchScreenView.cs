using App.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation
{
    public sealed class SearchScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _searchInputField;
        [SerializeField] private Button _searchButton;
        [SerializeField] private GameObject _idlePanel;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private GameObject _emptyPanel;
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private TextMeshProUGUI _errorMessageText;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private RectTransform _gridContent;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Button _loadMoreButton;
        [SerializeField] private GameObject _loadMoreSpinner;

        private SearchViewModel _viewModel;
        private IThumbnailRepository _thumbnailRepository;
        private int _renderedItemCount;

        public void Configure(
            TMP_InputField searchInputField,
            Button searchButton,
            GameObject idlePanel,
            GameObject loadingPanel,
            GameObject emptyPanel,
            GameObject errorPanel,
            TextMeshProUGUI errorMessageText,
            GameObject resultPanel,
            RectTransform gridContent,
            GameObject cardPrefab,
            Button loadMoreButton,
            GameObject loadMoreSpinner)
        {
            _searchInputField = searchInputField;
            _searchButton = searchButton;
            _idlePanel = idlePanel;
            _loadingPanel = loadingPanel;
            _emptyPanel = emptyPanel;
            _errorPanel = errorPanel;
            _errorMessageText = errorMessageText;
            _resultPanel = resultPanel;
            _gridContent = gridContent;
            _cardPrefab = cardPrefab;
            _loadMoreButton = loadMoreButton;
            _loadMoreSpinner = loadMoreSpinner;
        }

        public void Initialize(SearchViewModel viewModel, IThumbnailRepository thumbnailRepository)
        {
            _viewModel = viewModel;
            _thumbnailRepository = thumbnailRepository;
            _viewModel.Changed += Render;
            Render();
        }

        private void Awake()
        {
            _searchButton.onClick.AddListener(OnSearchClicked);
            _loadMoreButton.onClick.AddListener(OnLoadMoreClicked);
        }

        private void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.Changed -= Render;
            }
        }

        private void OnSearchClicked()
        {
            _viewModel.Search(_searchInputField.text);
        }

        private void OnLoadMoreClicked()
        {
            _viewModel.LoadMore();
        }

        private void Render()
        {
            var state = _viewModel.State;

            _idlePanel.SetActive(state == SearchScreenState.Idle);
            _loadingPanel.SetActive(state == SearchScreenState.Loading);
            _emptyPanel.SetActive(state == SearchScreenState.Empty);
            _errorPanel.SetActive(state == SearchScreenState.Error);
            _resultPanel.SetActive(state == SearchScreenState.Success || state == SearchScreenState.LoadingMore);

            if (state == SearchScreenState.Error)
            {
                _errorMessageText.text = _viewModel.ErrorMessage;
            }

            if (state == SearchScreenState.Success)
            {
                RebuildOrAppendGrid();
            }

            _loadMoreButton.gameObject.SetActive(state == SearchScreenState.Success && _viewModel.HasMore);
            _loadMoreSpinner.SetActive(state == SearchScreenState.LoadingMore);
        }

        private void RebuildOrAppendGrid()
        {
            var items = _viewModel.Items;

            if (_viewModel.LastPageWasFirstPage)
            {
                for (var i = _gridContent.childCount - 1; i >= 0; i--)
                {
                    Destroy(_gridContent.GetChild(i).gameObject);
                }
                _renderedItemCount = 0;
            }

            for (var i = _renderedItemCount; i < items.Count; i++)
            {
                var instance = Instantiate(_cardPrefab, _gridContent);
                instance.GetComponent<ImageCardView>().Bind(items[i], _thumbnailRepository);
            }
            _renderedItemCount = items.Count;
        }
    }
}
