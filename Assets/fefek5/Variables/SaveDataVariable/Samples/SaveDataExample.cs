using System.Collections.Generic;
using NoReleaseDate.Variables.SaveDataVariable.Runtime;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoReleaseDate.Variables.SaveDataVariable.Samples
{
    public class SaveDataExample : MonoBehaviour
    {
        [Button]
        public void TestSave()
        {
            var betterSaveData = new SaveData();
            betterSaveData.SetKey(SaveKey.RandomKey, 1)
                .SetKey(SaveKey.RandomKey.WithComment("This is 2"), 2)
                .SetKey(SaveKey.RandomKey, 3)
                .SetKey(SaveKey.RandomKey, Vector3Int.zero)
                .SetKey(SaveKey.RandomKey, Vector3Int.zero);

            var key1 = SaveKey.RandomKey.WithComment("This is for key 1");
            betterSaveData.GetKey(key1, new SaveData(), out var betterSaveData1);
            betterSaveData1.SetKey(SaveKey.RandomKey, 4)
                .SetKey(SaveKey.RandomKey, 5)
                .SetKey(SaveKey.RandomKey, 6);

            var key2 = SaveKey.RandomKey.WithComment("This is for key 2");
            betterSaveData1.GetKey(key2, new SaveData(), out var betterSaveData2);
            betterSaveData2.SetKey(SaveKey.RandomKey.WithComment("This is 7"), 7)
                .SetKey(SaveKey.RandomKey.WithComment("This is 8"), 8)
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

            var saveSettings = new SaveSettings() {
                JsonCustomSettings = new JsonSettings() {
                    FileComment = "Dawidek",
                }
            };
            
            betterSaveData.Save(Application.persistentDataPath + "/betterSaveData.sav", saveSettings);
        }

        [Button]
        public void TestLoad()
        {
            var betterSaveData = new SaveData();
            betterSaveData.Load(Application.persistentDataPath + "/betterSaveData.sav", onLoad: OnAfterLoad);
            
            return;
            
            void OnAfterLoad() => betterSaveData.Save(Application.persistentDataPath + "/betterSaveDataLoad.sav");
        }
    }
}
