using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fefek5.Common.Runtime.Extensions;
using fefek5.Systems.EncryptionSystem;
using fefek5.Variables.SaveDataVariable.Runtime.Settings;
using fefek5.Variables.SerializableGuidVariable.Runtime;
using Newtonsoft.Json;
using UnityEngine;

namespace fefek5.Variables.SaveDataVariable.Runtime
{
    /// <summary>
    /// SaveData class that can be used to save and load data
    /// </summary>
    [Serializable]
    public class SaveData
    {
        #region Properties

        /// <summary>
        /// Data that will be serialized to save file
        /// [SaveKey] - Is a key that contains string key and SerializableGuid key
        /// [object] - Is a value that will be saved
        /// </summary>
        public Dictionary<SaveKey, object> Data { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of BetterSaveData
        /// </summary>
        public SaveData() => Data = new Dictionary<SaveKey, object>();

        /// <summary>
        /// Create a new instance of BetterSaveData
        /// </summary>
        /// <param name="data">Data to be copied</param>
        public SaveData(Dictionary<SaveKey, object> data) => Data = new Dictionary<SaveKey, object>(data);

        /// <summary>
        /// Create a new instance of BetterSaveData
        /// </summary>
        /// <param name="saveData">SaveData to be copied</param>
        public SaveData(SaveData saveData) => Data = new Dictionary<SaveKey, object>(saveData.Data);

        #endregion
        
        #region Getters

        /// <summary>
        /// Get value from Data by string
        /// If key not found, default value will be returned but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value that will be found in Data or default value</returns>
        public T GetKey<T>(string saveKey, T defaultValue) => Data.GetAs(saveKey, defaultValue);

        /// <summary>
        /// Get value from Data by Guid
        /// If key not found, default value will be returned but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value that will be found in Data or default value</returns>
        public T GetKey<T>(Guid saveKey, T defaultValue) => Data.GetAs(saveKey, defaultValue);
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// If key not found, default value will be returned but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value that will be found in Data or default value</returns>
        public T GetKey<T>(SerializableGuid saveKey, T defaultValue) => Data.GetAs(saveKey, defaultValue);

        /// <summary>
        /// Get value from Data by SaveKey
        /// If key not found, default value will be returned but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value that will be found in Data or default value</returns>
        public T GetKey<T>(SaveKey saveKey, T defaultValue) => Data.GetAs(saveKey, defaultValue);

        /// <summary>
        /// Get value from Data by string
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(string saveKey, T defaultValue, out T value)
        {
            value = Data.GetAs(saveKey, defaultValue);
            return this;
        }
        
        /// <summary>
        /// Get value from Data by Guid
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(Guid saveKey, T defaultValue, out T value)
        {
            value = Data.GetAs(saveKey, defaultValue);
            return this;
        }
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(SerializableGuid saveKey, T defaultValue, out T value)
        {
            value = Data.GetAs(saveKey, defaultValue);
            return this;
        }
        
        /// <summary>
        /// Get value from Data by SaveKey
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(SaveKey saveKey, T defaultValue, out T value)
        {
            value = Data.GetAs(saveKey, defaultValue);
            return this;
        }
        
        /// <summary>
        /// Get value from Data by string
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(string saveKey, out T value)
        {
            if (IsKeyExist(saveKey))
            {
                value = Data.GetAs(saveKey, default(T));
                return true;
            }

            value = default;
            return false;
        }
        
        /// <summary>
        /// Get value from Data by Guid
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(Guid saveKey, out T value)
        {
            if (IsKeyExist(saveKey))
            {
                value = Data.GetAs(saveKey, default(T));
                return true;
            }

            value = default;
            return false;
        }
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(SerializableGuid saveKey, out T value)
        {
            if (IsKeyExist(saveKey))
            {
                value = Data.GetAs(saveKey, default(T));
                return true;
            }

            value = default;
            return false;
        }
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// If key not found, default value will be out but the key will be NOT added to Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(SaveKey saveKey, out T value)
        {
            if (IsKeyExist(saveKey))
            {
                value = Data.GetAs(saveKey, default(T));
                return true;
            }

            value = default;
            return false;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Set value to Data by string
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">Value to be set</param>
        /// <returns>This instance of SaveData</returns>
        public SaveData SetKey(string saveKey, object value)
        {
            Data[saveKey] = value;
            return this;
        }
        
        /// <summary>
        /// Set value to Data by SerializableGuid
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">Value to be set</param>
        /// <returns>This instance of SaveData</returns>
        public SaveData SetKey(SerializableGuid saveKey, object value)
        {
            Data[saveKey] = value;
            return this;
        }
        
        /// <summary>
        /// Set value to Data by Guid
        /// </summary>
        /// <param name="saveKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SaveData SetKey(Guid saveKey, object value)
        {
            Data[saveKey] = value;
            return this;
        }
        
        /// <summary>
        /// Set value to Data by SaveKey
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <param name="value">Value to be set</param>
        /// <returns>This instance of SaveData</returns>
        public SaveData SetKey(SaveKey saveKey, object value)
        {
            Data[saveKey] = value;
            return this;
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Get value from Data by string
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        public object this[string saveKey]
        {
            get => Data[saveKey];
            set => Data[saveKey] = value;
        }

        /// <summary>
        /// Get value from Data by Guid
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        public object this[Guid saveKey]
        {
            get => Data[saveKey];
            set => Data[saveKey] = value;
        }

        /// <summary>
        /// Get value from Data by SerializableGuid
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        public object this[SerializableGuid saveKey]
        {
            get => Data[saveKey];
            set => Data[saveKey] = value;
        }

        /// <summary>
        /// Get value from Data by SaveKey
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        public object this[SaveKey saveKey]
        {
            get => Data[saveKey];
            set => Data[saveKey] = value;
        }

        #endregion

        #region Helpers
        
        /// <summary>
        /// Check if key exist in Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(string saveKey) => Data.ContainsKey(saveKey);

        /// <summary>
        /// Check if key exist in Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(SerializableGuid saveKey) => Data.ContainsKey(saveKey);
        
        /// <summary>
        /// Check if saveKey exist in Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(Guid saveKey) => Data.ContainsKey(saveKey);
        
        /// <summary>
        /// Check if key exist in Data
        /// </summary>
        /// <param name="saveKey">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(SaveKey saveKey) => Data.ContainsKey(saveKey);

        /// <summary>
        /// Remove key from Data
        /// </summary>
        /// <param name="saveKey">Key to be removed</param>
        public void RemoveKey(string saveKey)
        {
            if (IsKeyExist(saveKey)) Data.Remove(saveKey);
        }

        /// <summary>
        /// Remove key from Data
        /// </summary>
        /// <param name="saveKey">Key to be removed</param>
        public void RemoveKey(SerializableGuid saveKey)
        {
            if (IsKeyExist(saveKey)) Data.Remove(saveKey);
        }
        
        /// <summary>
        /// Remove saveKey from Data
        /// </summary>
        /// <param name="saveKey">Key to be removed</param>
        public void RemoveKey(Guid saveKey)
        {
            if (IsKeyExist(saveKey)) Data.Remove(saveKey);
        }
        
        /// <summary>
        /// Remove saveKey from Data
        /// </summary>
        /// <param name="saveKey">Key to be removed</param>
        public void RemoveKey(SaveKey saveKey)
        {
            if (IsKeyExist(saveKey)) Data.Remove(saveKey);
        }

        #endregion

        #region Save | Load
        
        /// <summary>
        /// Save data to file
        /// </summary>
        /// <param name="path">Path to file</param>
        public void Save(string path) => Save(path, SaveSettings.Default);
        
        /// <summary>
        /// Save data to file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="saveSettings">Settings for saving</param>
        public void Save(string path, SaveSettings saveSettings) => Save(path, saveSettings, null);
        
        /// <summary>
        /// Save data to file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="onSave">Action that will be invoked after save completion</param>
        public void Save(string path, Action onSave) => Save(path, SaveSettings.Default, onSave);

        /// <summary>
        /// Save data to file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="saveSettings">Settings for saving</param>
        /// <param name="onSave">Action that will be invoked after save completion</param>
        public async void Save(string path, SaveSettings saveSettings, Action onSave)
        {
            try
            {
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

                    Debug.Log($"[SAVE-DATA] Saved to file: {path} "
                              + $"{directoryPath.ToFileLink("[Folder]")} "
                              + $"{path.ToFileLink("[File]")}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[SAVE-DATA] Error on save completion read the exception bellow");
                    Debug.LogError(e);
                }

                onSave?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log($"[SAVE-DATA] Error on save completion read the exception bellow");
                Debug.LogError(e);
            }
        }
        
        /// <summary>
        /// Load data from file
        /// </summary>
        /// <param name="path">Path to file</param>
        public void Load(string path) => Load(path, SaveSettings.Default);

        /// <summary>
        /// Load data from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="saveSettings">Settings for loading</param>
        public void Load(string path, SaveSettings saveSettings) => Load(path, saveSettings, null);
        
        /// <summary>
        /// Load data from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="onLoad">Action that will be invoked after load completion</param>
        public void Load(string path, Action onLoad) => Load(path, SaveSettings.Default, onLoad);

        /// <summary>
        /// Load data from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="saveSettings">Settings for loading</param>
        /// <param name="onLoad">Action that will be invoked after load completion</param>
        public async void Load(string path, SaveSettings saveSettings, Action onLoad)
        {
            try
            {
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
                        ? JsonConvert.DeserializeObject<SaveData>(jsonText, jsonSerializerSettings)
                        : new SaveData();

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

                onLoad?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log($"[SAVE-DATA] Error on load completion read the exception bellow");
                Debug.LogError(e);
            }
        }

        #endregion

        #region File Management
        
        /// <summary>
        /// Delete excess files in folder
        /// </summary>
        /// <param name="folderPath">Path to folder</param>
        /// <param name="fileLimit">Limit of files in folder</param>
        public static void DeleteExcessFiles(string folderPath, int fileLimit)
        {
            if (fileLimit <= 0) return;

            var saveFiles = GetSaveFiles(folderPath).SortOldestFirst().ToArray();
            var filesToDelete = saveFiles.Length - fileLimit;

            for (var i = 0; i < filesToDelete; i++)
                saveFiles[i].Delete();
        }

        /// <summary>
        /// Get save files from folder
        /// </summary>
        /// <param name="folderPath">Path to folder</param>
        /// <returns>Array of save files</returns>
        public static FileInfo[] GetSaveFiles(string folderPath) =>
            Directory.Exists(folderPath)
                ? new DirectoryInfo(folderPath).GetFiles("*.sav", SearchOption.AllDirectories)
                : null;

        #endregion
        
        #region ToJson

        /// <summary>
        /// Convert SaveData to Json string
        /// </summary>
        /// <returns>Json string</returns>
        public string ToJson() => ToJson(SaveSettings.Default);

        /// <summary>
        /// Convert SaveData to Json string
        /// </summary>
        /// <param name="saveSettings">Settings for serialization</param>
        /// <returns>Json string</returns>
        public string ToJson(SaveSettings saveSettings) =>
            saveSettings.UseJsonCustomSettings
                ? ToJson(saveSettings.JsonCustomSettings)
                : ToJson(new JsonSerializerSettings());

        /// <summary>
        /// Convert SaveData to Json string
        /// </summary>
        /// <param name="jsonSettings">Settings for serialization</param>
        /// <returns>Json string</returns>
        public string ToJson(JsonSettings jsonSettings) => ToJson(jsonSettings.JsonSerializerSettings);

        /// <summary>
        /// Convert SaveData to Json string
        /// </summary>
        /// <param name="jsonSerializerSettings">Settings for serialization</param>
        /// <returns>Json string</returns>
        public string ToJson(JsonSerializerSettings jsonSerializerSettings) =>
            JsonConvert.SerializeObject(new SaveData(this), jsonSerializerSettings);

        #endregion
    }
}