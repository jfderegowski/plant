using UnityEngine;
using UnityEngine.UI;

namespace fefek5.Systems.SaveSystem.Examples.SaveGameSystem.Scripts.UI
{
    public enum Navigation
    {
        All,
        Auto,
        Quick,
        Manual
    }
    
    public class UISavePanel : MonoBehaviour
    {
        public Navigation CurrentSelectedNavigation
        {
            get => _currentSelectedNavigation;
            set
            {
                if (_currentSelectedNavigation == value)
                    return;
                
                _currentSelectedNavigation = value;
                
                UpdateUI();
            }
        }
        
        [Header("UI Elements")]
        [SerializeField] private Transform _content;
        [SerializeField] private SaveSlotUIElement _saveSlotPrefab;
        
        [Header("Navigation")]
        [SerializeField] private Button _allButton;
        [SerializeField] private Button _autoButton;
        [SerializeField] private Button _quickButton;
        [SerializeField] private Button _manualButton;
        
        private Navigation _currentSelectedNavigation;

        private void Start()
        {
            _allButton.onClick.AddListener(() => CurrentSelectedNavigation = Navigation.All);
            _autoButton.onClick.AddListener(() => CurrentSelectedNavigation = Navigation.Auto);
            _quickButton.onClick.AddListener(() => CurrentSelectedNavigation = Navigation.Quick);
            _manualButton.onClick.AddListener(() => CurrentSelectedNavigation = Navigation.Manual);
            
            CurrentSelectedNavigation = Navigation.All;
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            // Debug.Log($"Current selected navigation: {CurrentSelectedNavigation}");
            //
            // var folderPath = CurrentSelectedNavigation switch {
            //     Navigation.All => SaveGameManager.CurrentProfileFolderPath,
            //     Navigation.Auto => SaveGameManager.CurrentAutoSaveFolderPath,
            //     Navigation.Quick => SaveGameManager.CurrentQuickSaveFolderPath,
            //     Navigation.Manual => SaveGameManager.CurrentManualSaveFolderPath,
            //     _ => SaveGameManager.CurrentProfileFolderPath
            // };
            //
            // var saveFiles = SaveGameManager.GetSaveFiles(folderPath);
            //
            // _content.DestroyChildren();
            //
            // var prevEnabled = _saveSlotPrefab.gameObject.activeSelf;
            // _saveSlotPrefab.gameObject.SetActive(false);
            // foreach (var saveFile in saveFiles)
            // {
            //     var clone = Instantiate(_saveSlotPrefab, _content);
            //     clone.Initialize(saveFile);
            //     clone.gameObject.SetActive(true);
            // }
            // _saveSlotPrefab.gameObject.SetActive(prevEnabled);
        }
    }
}
