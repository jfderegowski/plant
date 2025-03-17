using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NoReleaseDate.Common.Runtime.Extensions;
using NoReleaseDate.Systems.EncryptionSystem;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    [Serializable] [JsonConverter(typeof(BetterSaveDataJsonConverter))]
    public class BetterSaveData
    {
        public Dictionary<SaveKey, object> Data { get; private set; } = new();

        public BetterSaveData() { }

        public BetterSaveData Get<T>(SaveKey key, out T value, T defaultValue)
        {
            if (Data.TryGetValue(key, out var saveValue) && saveValue is T tValue)
            {
                value = tValue;
            }
            else
            {
                value = defaultValue;
            }

            return this;
        }

        public BetterSaveData Set<T>(SaveKey key, T value)
        {
            Data[key] = value;
            return this;
        }

        public async void Save(string path, SaveSettings saveSettings = null,
            Action onBeforeSave = null, Action onAfterSave = null)
        {
            try
            {
                onBeforeSave?.Invoke();

                try
                {
                    saveSettings ??= SaveSettings.Default;

                    var jsonString = ToJson(saveSettings);

                    if (saveSettings.UseEncryption)
                    {
                        var password = saveSettings.Encryption.Password;
                        var salt = saveSettings.Encryption.Salt;
                        var initVector = saveSettings.Encryption.InitVector;

                        jsonString = jsonString.Encrypt(password, salt, initVector);
                    }

                    // Create directory if it doesn't exist
                    var directoryPath = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);


                    await File.WriteAllTextAsync(path, jsonString);

                    // Delete excess files
                    if (saveSettings.UsedFileLimit)
                        DeleteExcessFiles(directoryPath, saveSettings.FileLimit);

                    Debug.Log($"[SAVE-DATA] Saved to File: {path} "
                              + $"{directoryPath.ToFileLink("[Folder]")} "
                              + $"{path.ToFileLink("[File]")}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[SAVE-DATA] Error on save completion read the exception bellow");
                    Debug.LogError(e);
                }

                onAfterSave?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log($"[SAVE-DATA] Error on save completion read the exception bellow");
                Debug.LogError(e);
            }
        }

        public async void Load(string path, SaveSettings saveSettings = null,
            Action onBeforeLoad = null, Action onAfterLoad = null)
        {
            try
            {
                onBeforeLoad?.Invoke();

                try
                {
                    saveSettings ??= SaveSettings.Default;

                    var jsonSerializerSettings = saveSettings.UseJsonCustomSettings
                        ? saveSettings.JsonCustomSettings.JsonSerializerSettings
                        : new JsonSerializerSettings();

                    var jsonText = await File.ReadAllTextAsync(path);

                    if (saveSettings.UseEncryption)
                    {
                        var password = saveSettings.Encryption.Password;
                        var salt = saveSettings.Encryption.Salt;
                        var initVector = saveSettings.Encryption.InitVector;

                        jsonText = jsonText.Decrypt(password, salt, initVector);
                    }

                    var saveData = !string.IsNullOrEmpty(jsonText)
                        ? JsonConvert.DeserializeObject<BetterSaveData>(jsonText, jsonSerializerSettings)
                        : new BetterSaveData();

                    Data = saveData.Data;

                    Debug.Log($"[SAVE-DATA] Loaded from File: {path} "
                              + $"{Path.GetDirectoryName(path).ToFileLink("[Folder]")} "
                              + $"{path.ToFileLink("[File]")}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[SAVE-DATA] Error on load completion read the exception bellow");
                    Debug.LogError(e);
                }

                onAfterLoad?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log($"[SAVE-DATA] Error on load completion read the exception bellow");
                Debug.LogError(e);
            }
        }

        public string ToJson() => ToJson(SaveSettings.Default);

        public string ToJson(SaveSettings saveSettings) =>
            saveSettings.UseJsonCustomSettings
                ? ToJson(saveSettings.JsonCustomSettings)
                : ToJson(new JsonSerializerSettings());

        public string ToJson(JsonSettings jsonSettings) => ToJson(jsonSettings.JsonSerializerSettings);

        public string ToJson(JsonSerializerSettings jsonSerializerSettings) =>
            JsonConvert.SerializeObject(new BetterSaveData { Data = Data }, jsonSerializerSettings);

        private static void DeleteExcessFiles(string folderPath, int fileLimit)
        {
            if (fileLimit <= 0) return;

            var saveFiles = GetSaveFiles(folderPath).SortOldestFirst().ToArray();
            var filesToDelete = saveFiles.Length - fileLimit;

            for (var i = 0; i < filesToDelete; i++)
                saveFiles[i].Delete();
        }

        private static FileInfo[] GetSaveFiles(string folderPath) =>
            Directory.Exists(folderPath)
                ? new DirectoryInfo(folderPath).GetFiles("*.sav", SearchOption.AllDirectories)
                : null;
    }
}