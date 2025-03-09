using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoReleaseDate.Plugins.SaveSystem.Examples.SaveGameSystem.Scripts.UI
{
    public class SaveSlotUIElement : MonoBehaviour
    {
        public FileInfo SaveFile { get; private set; }
        
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _saveName;
        [SerializeField] private TMP_Text _saveDate;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _deleteButton;
        
        private void OnEnable()
        {
            _loadButton.onClick.AddListener(OnLoadButtonClicked);
            _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }
        
        private void OnDisable()
        {
            _loadButton.onClick.RemoveListener(OnLoadButtonClicked);
            _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        }
        
        private void OnLoadButtonClicked() => Debug.Log("Load button clicked");

        private void OnDeleteButtonClicked()
        {
            SaveFile.Delete();
            Destroy(gameObject);
        }

        public void Initialize(FileInfo saveFile)
        {
            SaveFile = saveFile;
            
            _saveName.text = saveFile.Name;
            _saveDate.text = saveFile.LastWriteTime.ToString("dd/MM/yyyy HH:mm");
        }
    }
}