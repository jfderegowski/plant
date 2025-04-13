using System;
using fefek5.Variables.SaveDataVariable.Runtime;
using fefek5.Variables.SaveDataVariable.Runtime.Settings;
using fefek5.Variables.SerializableGuidVariable.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace fefek5.Variables.SaveDataVariable.Samples
{
    public class SaveDataExample : MonoBehaviour
    {
        [SerializeField] private SerializableGuid _guid = SerializableGuid.NewGuid();
        
        [SerializeField] private SaveKey _saveKey = SaveKey.RandomKey;
        
        [Button]
        public void TestSave()
        {
            var betterSaveData = new SaveData();
            betterSaveData.SetKey(SaveKey.RandomKey, 1)
                .SetKey(SaveKey.RandomKey.SetComment("This is 2"), 2)
                .SetKey(SaveKey.RandomKey, 3)
                .SetKey(SaveKey.RandomKey, Vector3Int.zero)
                .SetKey(SaveKey.RandomKey, Vector3Int.zero);
        
            var key1 = SaveKey.RandomKey.SetComment("This is for key 1");
            betterSaveData.GetKey(key1, new SaveData(), out var betterSaveData1);
            betterSaveData1.SetKey(SaveKey.RandomKey, 4)
                .SetKey(SaveKey.RandomKey, 5)
                .SetKey(SaveKey.RandomKey, 6);
        
            var key2 = SaveKey.RandomKey.SetComment("This is for key 2");
            betterSaveData1.GetKey(key2, new SaveData(), out var betterSaveData2);
            betterSaveData2.SetKey(SaveKey.RandomKey.SetComment("This is 7"), 7)
                .SetKey(SaveKey.RandomKey.SetComment("This is 8"), 8)
                .SetKey(SaveKey.RandomKey, 9)
                .SetKey(SaveKey.RandomKey, Vector3Int.one);
            
            betterSaveData1.SetKey(key2, betterSaveData2);
            betterSaveData.SetKey(key1, betterSaveData1)
                .SetKey(SaveKey.RandomKey, 10)
                .SetKey(SaveKey.RandomKey, 11)
                .SetKey(SaveKey.RandomKey, 12)
                .SetKey(SaveKey.RandomKey, Vector3Int.one)
                .SetKey(SaveKey.RandomKey, Vector3Int.zero)
                .SetKey(SaveKey.RandomKey, "After BetterSaveData");
            
            betterSaveData.Save(Application.persistentDataPath + "/betterSaveData.sav");
        }
        
        [Button]
        public void TestLoad()
        {
            var betterSaveData = new SaveData();
            betterSaveData.Load(Application.persistentDataPath + "/betterSaveData.sav", onLoad: OnAfterLoad);
            
            return;
            
            void OnAfterLoad() => betterSaveData.Save(Application.persistentDataPath + "/betterSaveDataLoad.sav");
        }

        [Button]
        public void TestArrayGetters()
        {
            var key1 = "StringKey";
            var key2 = SerializableGuid.NewGuid().ToHexString();
            var key3 = Guid.NewGuid();
            var key4 = SaveKey.RandomKey;
            
            var saveData = new SaveData();

            saveData[key1] = 1;
            saveData[key2] = 2;
            saveData[key3] = 3;
            saveData[key4] = 4;
            
            var path = Application.persistentDataPath + "/testSaveDataThisGetters.sav";
            
            saveData.Save(path);
            
            saveData.Load(path, OnLoad);
            
            return;
            
            void OnLoad()
            {
                Debug.Log($"Key1 - " +
                          $"Value : {saveData[key1]}");
                
                Debug.Log($"Key2 - " +
                          $"Value : {saveData[key2]}");
                
                Debug.Log($"Key3 - " +
                          $"Value : {saveData[key3]}");
                
                Debug.Log($"Key4 - " +
                          $"Value : {saveData[key4]}");
            }
        }
    }
}
