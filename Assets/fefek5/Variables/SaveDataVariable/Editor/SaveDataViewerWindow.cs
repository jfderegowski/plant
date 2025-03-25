using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace NoReleaseDate.Variables.SaveDataVariable.Editor
{
    public class SaveDataViewerWindow : EditorWindow
    {
        private ScrollView _jsonScrollView;
        private TextField _searchTextField;
        private Label _counterLabel;
        private string _originalJsonContent;
        private List<VisualElement> _highlightedElements = new List<VisualElement>();
        private int _currentHighlightIndex = -1;

        public static void ShowWindow(string jsonContent)
        {
            var window = GetWindow<SaveDataViewerWindow>();
            window.titleContent = new GUIContent("SaveData Viewer");
            window.minSize = new Vector2(400, 300);
            window.SetJsonContent(jsonContent);

            Debug.Log(jsonContent);
        }

        public void SetJsonContent(string jsonContent)
        {
            _originalJsonContent = jsonContent.Replace("\r\n", "\n");
            UpdateJsonDisplay();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.marginLeft = 1;
            root.style.marginRight = 1;

            var searchContainer = new VisualElement { 
                style = { 
                    flexDirection = FlexDirection.Row,
                    marginBottom = 5,
                    alignItems = Align.Center
                }
            };

            _searchTextField = new TextField { 
                style = { flexGrow = 1, marginRight = 5 } 
            };
            _searchTextField.RegisterValueChangedCallback(evt => UpdateJsonDisplay());

            _counterLabel = new Label("0/0") { 
                style = { 
                    marginRight = 5,
                    minWidth = 50,
                    unityTextAlign = TextAnchor.MiddleCenter
                } 
            };

            var prevButton = new Button(() => Navigate(-1)) { 
                text = "<", 
                tooltip = "Previous", 
                style = { width = 30 } 
            };
            
            var nextButton = new Button(() => Navigate(1)) { 
                text = ">", 
                tooltip = "Next", 
                style = { width = 30 } 
            };

            searchContainer.Add(_searchTextField);
            searchContainer.Add(prevButton);
            searchContainer.Add(_counterLabel);
            searchContainer.Add(nextButton);

            _jsonScrollView = new ScrollView();
            root.Add(searchContainer);
            root.Add(_jsonScrollView);
        }

        private void UpdateJsonDisplay()
        {
            _jsonScrollView.Clear();
            _highlightedElements.Clear();
            _currentHighlightIndex = -1;

            if (string.IsNullOrEmpty(_originalJsonContent)) return;

            var searchText = _searchTextField.value;
            var lines = _originalJsonContent.Split('\n');

            foreach (var line in lines)
            {
                var lineContainer = new VisualElement();
                lineContainer.style.flexDirection = FlexDirection.Row;
                lineContainer.style.flexWrap = Wrap.NoWrap;
                lineContainer.style.marginLeft = 0;
                lineContainer.style.marginRight = 0;

                if (string.IsNullOrEmpty(searchText))
                {
                    AddPreservedWhitespaceLabel(lineContainer, line);
                }
                else
                {
                    var parts = line.Split(new[] { searchText }, System.StringSplitOptions.None);
                    
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(parts[i]))
                        {
                            AddPreservedWhitespaceLabel(lineContainer, parts[i]);
                        }

                        if (i < parts.Length - 1)
                        {
                            var highlight = CreateHighlight(searchText);
                            _highlightedElements.Add(highlight);
                            lineContainer.Add(highlight);
                        }
                    }
                }

                _jsonScrollView.Add(lineContainer);
            }

            UpdateCounter();
            if (_highlightedElements.Count > 0) Navigate(0);
        }

        private void AddPreservedWhitespaceLabel(VisualElement container, string text)
        {
            var label = new Label(text);
            label.style.whiteSpace = WhiteSpace.Pre;
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            container.Add(label);
        }

        private VisualElement CreateHighlight(string text)
        {
            var highlight = new Label(text);
            highlight.style.backgroundColor = new Color(1, 1, 0, 0.3f);
            highlight.style.unityTextAlign = TextAnchor.MiddleLeft;
            highlight.style.whiteSpace = WhiteSpace.Pre;
            highlight.style.paddingLeft = 0;
            highlight.style.paddingRight = 0;
            highlight.style.marginLeft = 0;
            highlight.style.marginRight = 0;
            return highlight;
        }

        private void UpdateCounter()
        {
            _counterLabel.text = $"{(_currentHighlightIndex >= 0 ? _currentHighlightIndex + 1 : 0)}/{_highlightedElements.Count}";
        }

        private void Navigate(int direction)
        {
            if (_highlightedElements.Count == 0) return;

            if (_currentHighlightIndex >= 0)
            {
                _highlightedElements[_currentHighlightIndex].style.backgroundColor = new Color(1, 1, 0, 0.3f);
            }

            _currentHighlightIndex = (_currentHighlightIndex + direction + _highlightedElements.Count) % _highlightedElements.Count;
            
            var element = _highlightedElements[_currentHighlightIndex];
            element.style.backgroundColor = new Color(1, 0.5f, 0, 0.6f);
            element.schedule.Execute(() => _jsonScrollView.ScrollTo(element)).StartingIn(0);
            UpdateCounter();
        }
    }
}