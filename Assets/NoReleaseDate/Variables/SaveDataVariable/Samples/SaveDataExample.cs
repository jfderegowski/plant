using System.Collections.Generic;
using NoReleaseDate.Variables.SaveDataVariable.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Samples
{
    public class SaveDataExample : MonoBehaviour
    {
        [SerializeField] private SaveData _emptySaveData;
        [SerializeField] private SaveData _saveData;
        [SerializeField] private SaveKey _saveKey;
        [SerializeField] private SaveKey _saveKeyForList;
        [SerializeField] private List<string> _list = new();
        [SerializeField] private BetterSaveData _betterSaveData;
        
        [Button]
        private void Save()
        {
            _saveData.SetKey(_saveKey, GetClasDataFromList(_list));
        }

        private ClassData GetClasDataFromList(List<string> list)
        {
            var clasData = new ClassData();
            
            clasData.SetKey(_saveKeyForList, list);
            
            return clasData;
        }
            
        [Button]
        private void Load()
        {
            _list = _saveData
                .GetKey(_saveKey, GetDefaultClassData())
                .GetKey(_saveKeyForList, new List<string>());
        }

        [Button]
        private void SaveToFile() => 
            _saveData.Save(Application.persistentDataPath + "/saveData.sav");
        
        [Button]
        private void LoadFromFile() => 
            _saveData.Load(Application.persistentDataPath + "/saveData.sav");

        private ClassData GetDefaultClassData()
        {
            var clasData = new ClassData();
            
            clasData.SetKey(_saveKeyForList, new List<string>());
            
            return clasData;
        }

        [Button]
        private void TestSave()
        {
            _betterSaveData = new BetterSaveData();
            _betterSaveData.Set(SaveKey.RandomKey, 1)
                .Set(SaveKey.RandomKey.WithComment("This is 2"), 2)
                .Set(SaveKey.RandomKey, 3);
            
            var key1 = SaveKey.RandomKey.WithComment("This is for key 1");
            _betterSaveData.Get(key1, out var betterSaveData1, new BetterSaveData());
            betterSaveData1.Set(SaveKey.RandomKey, 4)
                .Set(SaveKey.RandomKey, 5)
                .Set(SaveKey.RandomKey, 6);

            var key2 = SaveKey.RandomKey.WithComment("This is for key 2");
            betterSaveData1.Get(key2, out var betterSaveData2, new BetterSaveData());
            betterSaveData2.Set(SaveKey.RandomKey.WithComment("This is 7"), 7)
                .Set(SaveKey.RandomKey.WithComment("This is 8"), 8)
                .Set(SaveKey.RandomKey, 9)
                .Set(SaveKey.RandomKey, Vector3Int.one);
            
            betterSaveData1.Set(key2, betterSaveData2);
            _betterSaveData.Set(key1, betterSaveData1);
            
            _betterSaveData.Save(Application.persistentDataPath + "/betterSaveData.sav");
        }

        [Button]
        private void TestLoad()
        {
            _betterSaveData.Load(Application.persistentDataPath + "/betterSaveData.sav");
        }
    }
}
