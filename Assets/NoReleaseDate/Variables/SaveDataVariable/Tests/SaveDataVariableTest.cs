using System.Threading.Tasks;
using NoReleaseDate.Variables.SaveDataVariable.Runtime;
using NoReleaseDate.Variables.SerializableGuidVariable.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Tests
{
    public class SaveDataVariableTest
    {
        [Test]
        public async Task SaveDataVariableTestSimplePasses()
        {
            var isTestComplete = false;
            var saveData = new SaveData();
            var classData = new ClassData();
            var savePath = $"{Application.persistentDataPath}/TestRunner/SaveDataVariableTest/save.sav";
    
            // Test with random key
            var saveKeyRandom = SaveKey.RandomKey;
            var classKeyRandom = SaveKey.RandomKey;
            classData.SetKey(classKeyRandom, "Value Random Key");
            saveData.SetKey(saveKeyRandom, classData);
            saveData.Save(savePath, onAfterSave: OnSaveComplete);
            await WaitForCompletion();
    
            // Test with string key
            var saveKeyString = new SaveKey("StringKey");
            classData.SetKey(saveKeyString, "Value String Key");
            saveData.SetKey(saveKeyString, classData);
            saveData.Save(savePath, onAfterSave: OnSaveComplete);
            await WaitForCompletion();
    
            // Test with SerializableGuid key
            var saveKeyGuid = new SaveKey(SerializableGuid.NewGuid());
            classData.SetKey(saveKeyGuid, "Value GUID Key");
            saveData.SetKey(saveKeyGuid, classData);
            saveData.Save(savePath, onAfterSave: OnSaveComplete);
            await WaitForCompletion();
    
            return;
    
            void OnSaveComplete() => isTestComplete = true;
            async Task WaitForCompletion() { while (!isTestComplete) await Task.Yield(); isTestComplete = false; }
        }
    }
}
