using System;
using System.Reflection;
using App.Composition;
using App.Composition.Mock;
using App.Presentation;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace App.EditorTools
{
    // Regenerates the Search screen (scene + reusable card prefab) from scratch.
    // Tools > UI > Generate Search Screen
    public static class SearchScreenBuilder
    {
        private const string ScenePath = "Assets/Scenes/Search.unity";
        private const string CardPrefabPath = "Assets/Prefabs/UI/ImageCard.prefab";
        private const string FontAssetPath = "Assets/Fonts/MaplestoryLight SDF.asset";
        private const string GameViewSizeLabel = "Portrait 1080x1920";

        [MenuItem("Tools/UI/Generate Search Screen")]
        public static void GenerateSearchScreen()
        {
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            if (fontAsset == null)
            {
                Debug.LogWarning("Korean font asset not found at " + FontAssetPath + " — text will use the default TMP font.");
            }

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var canvasGO = BuildCanvas();
            var safeAreaGO = BuildSafeArea(canvasGO.transform);
            var headerGO = BuildHeader(safeAreaGO.transform, fontAsset);
            var stateContentGO = BuildStateContent(safeAreaGO.transform, fontAsset);
            BuildEventSystem();

            var cardPrefab = EnsureCardPrefab(fontAsset);
            WireSearchScreen(safeAreaGO, headerGO, stateContentGO, cardPrefab);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(scene, ScenePath);

            SetGameViewSize();

            Debug.Log("Search screen generated at " + ScenePath);
        }

        private static GameObject BuildCanvas()
        {
            var canvasGO = new GameObject("Canvas", typeof(RectTransform));
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();
            return canvasGO;
        }

        private static GameObject BuildSafeArea(Transform parent)
        {
            var safeAreaGO = new GameObject("SafeArea", typeof(RectTransform));
            safeAreaGO.transform.SetParent(parent, false);
            var rt = safeAreaGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            safeAreaGO.AddComponent<SafeAreaFitter>();
            return safeAreaGO;
        }

        private static void BuildEventSystem()
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        private static GameObject BuildHeader(Transform parent, TMP_FontAsset font)
        {
            var headerGO = new GameObject("Header", typeof(RectTransform));
            headerGO.transform.SetParent(parent, false);
            var headerRT = headerGO.GetComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0f, 1f);
            headerRT.anchorMax = new Vector2(1f, 1f);
            headerRT.pivot = new Vector2(0.5f, 1f);
            headerRT.sizeDelta = new Vector2(0f, 120f);
            headerRT.anchoredPosition = Vector2.zero;

            var headerLayout = headerGO.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(24, 24, 12, 12);
            headerLayout.spacing = 16f;
            headerLayout.childAlignment = TextAnchor.MiddleLeft;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            BuildSearchInputField(headerGO.transform, font);
            BuildSearchButton(headerGO.transform, font);
            return headerGO;
        }

        private static void BuildSearchInputField(Transform parent, TMP_FontAsset font)
        {
            var inputGO = new GameObject("SearchInputField", typeof(RectTransform));
            inputGO.transform.SetParent(parent, false);
            inputGO.AddComponent<Image>().color = Color.white;
            var inputField = inputGO.AddComponent<TMP_InputField>();
            inputGO.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var textArea = new GameObject("Text Area", typeof(RectTransform));
            textArea.transform.SetParent(inputGO.transform, false);
            var textAreaRT = textArea.GetComponent<RectTransform>();
            textAreaRT.anchorMin = Vector2.zero;
            textAreaRT.anchorMax = Vector2.one;
            textAreaRT.offsetMin = new Vector2(12f, 6f);
            textAreaRT.offsetMax = new Vector2(-12f, -6f);
            textArea.AddComponent<RectMask2D>();

            var placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
            placeholderGO.transform.SetParent(textArea.transform, false);
            StretchFill(placeholderGO.GetComponent<RectTransform>());
            var placeholderText = placeholderGO.AddComponent<TextMeshProUGUI>();
            placeholderText.text = "검색어를 입력하세요";
            placeholderText.fontSize = 36;
            placeholderText.color = new Color(0.55f, 0.55f, 0.55f, 1f);
            placeholderText.alignment = TextAlignmentOptions.MidlineLeft;
            placeholderText.raycastTarget = false;
            if (font != null) placeholderText.font = font;

            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(textArea.transform, false);
            StretchFill(textGO.GetComponent<RectTransform>());
            var inputText = textGO.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 36;
            inputText.color = Color.black;
            inputText.alignment = TextAlignmentOptions.MidlineLeft;
            if (font != null) inputText.font = font;

            inputField.textViewport = textAreaRT;
            inputField.textComponent = inputText;
            inputField.placeholder = placeholderText;
            inputField.characterLimit = 100;
            inputField.lineType = TMP_InputField.LineType.SingleLine;
        }

        private static void BuildSearchButton(Transform parent, TMP_FontAsset font)
        {
            var buttonGO = new GameObject("SearchButton", typeof(RectTransform));
            buttonGO.transform.SetParent(parent, false);
            buttonGO.AddComponent<Image>().color = new Color(0.16f, 0.47f, 0.95f, 1f);
            buttonGO.AddComponent<Button>();
            var le = buttonGO.AddComponent<LayoutElement>();
            le.preferredWidth = 160f;
            le.flexibleWidth = 0f;

            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(buttonGO.transform, false);
            StretchFill(labelGO.GetComponent<RectTransform>());
            var label = labelGO.AddComponent<TextMeshProUGUI>();
            label.text = "검색";
            label.fontSize = 36;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            if (font != null) label.font = font;
        }

        private static GameObject BuildStateContent(Transform parent, TMP_FontAsset font)
        {
            var stateContentGO = new GameObject("StateContent", typeof(RectTransform));
            stateContentGO.transform.SetParent(parent, false);
            var rt = stateContentGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = new Vector2(0f, -120f);

            var idlePanel = BuildMessagePanel(stateContentGO.transform, "IdlePanel", "검색어를 입력하고 검색 버튼을 눌러주세요", font);
            var loadingPanel = BuildMessagePanel(stateContentGO.transform, "LoadingPanel", "검색 중", font);
            var emptyPanel = BuildMessagePanel(stateContentGO.transform, "EmptyPanel", "결과 없음", font);
            var errorPanel = BuildMessagePanel(stateContentGO.transform, "ErrorPanel", "오류가 발생했습니다", font);
            var resultPanel = BuildResultPanel(stateContentGO.transform, font);

            loadingPanel.SetActive(false);
            emptyPanel.SetActive(false);
            errorPanel.SetActive(false);
            resultPanel.SetActive(false);
            idlePanel.SetActive(true);

            return stateContentGO;
        }

        private static GameObject BuildMessagePanel(Transform parent, string name, string message, TMP_FontAsset font)
        {
            var panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            StretchFill(panel.GetComponent<RectTransform>());

            var textGO = new GameObject("MessageText", typeof(RectTransform));
            textGO.transform.SetParent(panel.transform, false);
            var textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(40f, 40f);
            textRT.offsetMax = new Vector2(-40f, -40f);
            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = message;
            text.fontSize = 40;
            text.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            if (font != null) text.font = font;

            return panel;
        }

        private static GameObject BuildResultPanel(Transform parent, TMP_FontAsset font)
        {
            var resultPanel = new GameObject("ResultPanel", typeof(RectTransform));
            resultPanel.transform.SetParent(parent, false);
            StretchFill(resultPanel.GetComponent<RectTransform>());

            var scrollViewGO = new GameObject("ScrollView", typeof(RectTransform));
            scrollViewGO.transform.SetParent(resultPanel.transform, false);
            var scrollViewRT = scrollViewGO.GetComponent<RectTransform>();
            scrollViewRT.anchorMin = Vector2.zero;
            scrollViewRT.anchorMax = Vector2.one;
            scrollViewRT.offsetMin = new Vector2(0f, 80f);
            scrollViewRT.offsetMax = Vector2.zero;
            var scrollRect = scrollViewGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            var viewportGO = new GameObject("Viewport", typeof(RectTransform));
            viewportGO.transform.SetParent(scrollViewGO.transform, false);
            StretchFill(viewportGO.GetComponent<RectTransform>());
            viewportGO.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
            viewportGO.AddComponent<Mask>().showMaskGraphic = false;

            var gridContentGO = new GameObject("GridContent", typeof(RectTransform));
            gridContentGO.transform.SetParent(viewportGO.transform, false);
            var gridContentRT = gridContentGO.GetComponent<RectTransform>();
            gridContentRT.anchorMin = new Vector2(0f, 1f);
            gridContentRT.anchorMax = new Vector2(1f, 1f);
            gridContentRT.pivot = new Vector2(0.5f, 1f);
            gridContentRT.sizeDelta = Vector2.zero;
            gridContentRT.anchoredPosition = Vector2.zero;
            var gridLayout = gridContentGO.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(512f, 608f);
            gridLayout.spacing = new Vector2(16f, 16f);
            gridLayout.padding = new RectOffset(20, 20, 20, 20);
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 2;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            var fitter = gridContentGO.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            scrollRect.viewport = viewportGO.GetComponent<RectTransform>();
            scrollRect.content = gridContentRT;

            BuildLoadMoreRow(resultPanel.transform, font);
            return resultPanel;
        }

        private static void BuildLoadMoreRow(Transform parent, TMP_FontAsset font)
        {
            var row = new GameObject("LoadMoreRow", typeof(RectTransform));
            row.transform.SetParent(parent, false);
            var rowRT = row.GetComponent<RectTransform>();
            rowRT.anchorMin = new Vector2(0f, 0f);
            rowRT.anchorMax = new Vector2(1f, 0f);
            rowRT.pivot = new Vector2(0.5f, 0f);
            rowRT.sizeDelta = new Vector2(0f, 80f);
            rowRT.anchoredPosition = Vector2.zero;

            var buttonGO = new GameObject("LoadMoreButton", typeof(RectTransform));
            buttonGO.transform.SetParent(row.transform, false);
            var buttonRT = buttonGO.GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRT.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRT.pivot = new Vector2(0.5f, 0.5f);
            buttonRT.sizeDelta = new Vector2(280f, 64f);
            buttonRT.anchoredPosition = Vector2.zero;
            buttonGO.AddComponent<Image>().color = new Color(0.85f, 0.85f, 0.85f, 1f);
            buttonGO.AddComponent<Button>();

            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(buttonGO.transform, false);
            StretchFill(labelGO.GetComponent<RectTransform>());
            var label = labelGO.AddComponent<TextMeshProUGUI>();
            label.text = "더보기";
            label.fontSize = 32;
            label.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            if (font != null) label.font = font;

            var spinnerGO = new GameObject("LoadMoreSpinnerText", typeof(RectTransform));
            spinnerGO.transform.SetParent(row.transform, false);
            StretchFill(spinnerGO.GetComponent<RectTransform>());
            var spinnerText = spinnerGO.AddComponent<TextMeshProUGUI>();
            spinnerText.text = "불러오는 중";
            spinnerText.fontSize = 32;
            spinnerText.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            spinnerText.alignment = TextAlignmentOptions.Center;
            spinnerText.raycastTarget = false;
            if (font != null) spinnerText.font = font;
            spinnerGO.SetActive(false);
        }

        private static GameObject EnsureCardPrefab(TMP_FontAsset font)
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(CardPrefabPath);
            if (existing != null)
            {
                if (existing.GetComponent<ImageCardView>() == null)
                {
                    var contents = PrefabUtility.LoadPrefabContents(CardPrefabPath);
                    contents.AddComponent<ImageCardView>();
                    PrefabUtility.SaveAsPrefabAsset(contents, CardPrefabPath);
                    PrefabUtility.UnloadPrefabContents(contents);
                    existing = AssetDatabase.LoadAssetAtPath<GameObject>(CardPrefabPath);
                }
                return existing;
            }

            var cardGO = new GameObject("ImageCard", typeof(RectTransform));
            var cardRT = cardGO.GetComponent<RectTransform>();
            cardRT.sizeDelta = new Vector2(512f, 608f);
            cardGO.AddComponent<Image>().color = new Color(0.95f, 0.95f, 0.95f, 1f);
            var vlg = cardGO.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(8, 8, 8, 8);
            vlg.spacing = 4f;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var thumbGO = new GameObject("Thumbnail", typeof(RectTransform));
            thumbGO.transform.SetParent(cardGO.transform, false);
            var thumbImage = thumbGO.AddComponent<Image>();
            thumbImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            thumbImage.raycastTarget = false;
            thumbGO.AddComponent<LayoutElement>().preferredHeight = 496f;

            var tagsGO = new GameObject("TagsText", typeof(RectTransform));
            tagsGO.transform.SetParent(cardGO.transform, false);
            var tagsText = tagsGO.AddComponent<TextMeshProUGUI>();
            tagsText.fontSize = 24f;
            tagsText.color = new Color(0.25f, 0.25f, 0.25f, 1f);
            tagsText.alignment = TextAlignmentOptions.MidlineLeft;
            tagsText.text = "tag1, tag2, tag3";
            tagsText.textWrappingMode = TextWrappingModes.Normal;
            tagsText.overflowMode = TextOverflowModes.Ellipsis;
            tagsText.raycastTarget = false;
            if (font != null) tagsText.font = font;
            tagsGO.AddComponent<LayoutElement>().preferredHeight = 40f;

            var authorGO = new GameObject("AuthorText", typeof(RectTransform));
            authorGO.transform.SetParent(cardGO.transform, false);
            var authorText = authorGO.AddComponent<TextMeshProUGUI>();
            authorText.fontSize = 20f;
            authorText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            authorText.alignment = TextAlignmentOptions.MidlineLeft;
            authorText.text = "author";
            authorText.raycastTarget = false;
            if (font != null) authorText.font = font;
            authorGO.AddComponent<LayoutElement>().preferredHeight = 32f;

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
            {
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
            }

            cardGO.AddComponent<ImageCardView>();

            var prefab = PrefabUtility.SaveAsPrefabAsset(cardGO, CardPrefabPath, out _);
            UnityEngine.Object.DestroyImmediate(cardGO);
            return prefab;
        }

        // Wires the screen's UI elements into a SearchScreenView and builds the composition
        // root (Mock DataSources -> Repositories -> ViewModel -> View) so the screen is a
        // fully working app screen, backed by a Mock DataSource instead of the real API.
        private static void WireSearchScreen(GameObject safeAreaGO, GameObject headerGO, GameObject stateContentGO, GameObject cardPrefab)
        {
            var searchInputField = headerGO.transform.Find("SearchInputField").GetComponent<TMP_InputField>();
            var searchButton = headerGO.transform.Find("SearchButton").GetComponent<Button>();

            var idlePanel = stateContentGO.transform.Find("IdlePanel").gameObject;
            var loadingPanel = stateContentGO.transform.Find("LoadingPanel").gameObject;
            var emptyPanel = stateContentGO.transform.Find("EmptyPanel").gameObject;
            var errorPanel = stateContentGO.transform.Find("ErrorPanel").gameObject;
            var errorMessageText = errorPanel.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
            var resultPanel = stateContentGO.transform.Find("ResultPanel").gameObject;
            var gridContent = (RectTransform)resultPanel.transform.Find("ScrollView/Viewport/GridContent");
            var loadMoreButton = resultPanel.transform.Find("LoadMoreRow/LoadMoreButton").GetComponent<Button>();
            var loadMoreSpinner = resultPanel.transform.Find("LoadMoreRow/LoadMoreSpinnerText").gameObject;

            var view = safeAreaGO.AddComponent<SearchScreenView>();
            view.Configure(
                searchInputField,
                searchButton,
                idlePanel,
                loadingPanel,
                emptyPanel,
                errorPanel,
                errorMessageText,
                resultPanel,
                gridContent,
                cardPrefab,
                loadMoreButton,
                loadMoreSpinner);

            var compositionGO = new GameObject("Composition");
            var mockSearchDataSource = compositionGO.AddComponent<MockPixabaySearchDataSource>();
            var mockThumbnailDataSource = compositionGO.AddComponent<MockThumbnailDataSource>();
            var installer = compositionGO.AddComponent<SearchScreenInstaller>();
            installer.Configure(mockSearchDataSource, mockThumbnailDataSource, view);
        }

        private static void StretchFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Best-effort: adds/selects a 1080x1920 entry in the Game View's resolution dropdown.
        // Uses undocumented internal Editor APIs via reflection, so failure here never blocks
        // the rest of the generation — it only affects what the Game View tab shows.
        private static void SetGameViewSize()
        {
            try
            {
                var editorAssembly = typeof(Editor).Assembly;
                var sizesType = editorAssembly.GetType("UnityEditor.GameViewSizes");
                var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                var sizesInstance = singleType.GetProperty("instance")?.GetValue(null, null);

                var currentGroupType = sizesType.GetProperty("currentGroupType")?.GetValue(sizesInstance, null);
                var group = sizesType.GetMethod("GetGroup")?.Invoke(sizesInstance, new[] { currentGroupType });
                var groupType = editorAssembly.GetType("UnityEditor.GameViewSizeGroup");

                var getDisplayTexts = groupType.GetMethod("GetDisplayTexts");
                var displayTexts = (string[])getDisplayTexts.Invoke(group, null);
                var index = Array.FindIndex(displayTexts, t => t.StartsWith(GameViewSizeLabel, StringComparison.Ordinal));

                if (index < 0)
                {
                    var sizeType = editorAssembly.GetType("UnityEditor.GameViewSize");
                    var sizeTypeEnum = editorAssembly.GetType("UnityEditor.GameViewSizeType");
                    var fixedResolution = Enum.Parse(sizeTypeEnum, "FixedResolution");
                    var ctor = sizeType.GetConstructor(new[] { sizeTypeEnum, typeof(int), typeof(int), typeof(string) });
                    var newSize = ctor?.Invoke(new object[] { fixedResolution, 1080, 1920, GameViewSizeLabel });
                    groupType.GetMethod("AddCustomSize")?.Invoke(group, new[] { newSize });

                    displayTexts = (string[])getDisplayTexts.Invoke(group, null);
                    index = Array.FindIndex(displayTexts, t => t.StartsWith(GameViewSizeLabel, StringComparison.Ordinal));
                }

                var gameViewType = editorAssembly.GetType("UnityEditor.GameView");
                var gameViewWindow = EditorWindow.GetWindow(gameViewType, false, null, false);
                var sizeIndexProp = gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                sizeIndexProp?.SetValue(gameViewWindow, index, null);
                gameViewWindow.Repaint();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not set the Game View size to 1080x1920 automatically (internal Editor API may have changed): " + ex.Message);
            }
        }
    }
}
