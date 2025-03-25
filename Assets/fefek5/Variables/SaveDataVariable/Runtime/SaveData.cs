using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NoReleaseDate.Common.Runtime.Extensions;
using NoReleaseDate.Systems.EncryptionSystem;
using NoReleaseDate.Variables.HasValueVariable.Runtime;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using NoReleaseDate.Variables.SerializableGuidVariable.Runtime;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    /// <summary>
    /// SaveData class that can be used to save and load data
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>
        /// Data that will be serialized to save file
        /// [SaveKey] - Is a key that contains string key and SerializableGuid key
        /// [object] - Is a value that will be saved
        /// </summary>
        public Dictionary<SaveKey, object> Data { get; private set; }

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
        /// Get value from Data by StringKey
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(string key, T defaultValue, out T value)
        {
            if (SerializableGuid.IsHexString(key))
                return GetKey(SerializableGuid.FromHexString(key), defaultValue, out value);

            foreach (var saveKey in Data.Keys)
            {
                if (saveKey != key) continue;
                
                if (Data[saveKey] is T tValue)
                {
                    value = tValue;
                    
                    return this;
                }
                    
                break;
            }

            value = defaultValue;

            return this;
        }
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(SerializableGuid key, T defaultValue, out T value)
        {
            foreach (var saveKey in Data.Keys)
            {
                if (saveKey != key) continue;

                if (Data[saveKey] is T tValue)
                {
                    value = tValue;
                
                    return this;
                }
                
                break;
            }

            value = defaultValue;

            return this;
        }
        
        /// <summary>
        /// Get value from Data by SaveKey
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="defaultValue">Default value that will be returned if key is not found</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>This instance of SaveData</returns>
        public SaveData GetKey<T>(SaveKey key, T defaultValue, out T value)
        {
            if (Data.TryGetValue(key, out var outValue) && outValue is T tValue)
            {
                value = tValue;
                return this;
            }
            
            value = defaultValue;
            return this;
        }
        
        /// <summary>
        /// Get value from Data by StringKey
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(string key, out T value)
        {
            if (SerializableGuid.IsHexString(key))
                return TryGetKey(SerializableGuid.FromHexString(key), out value);

            foreach (var saveKey in Data.Keys)
            {
                if (saveKey != key) continue;

                if (Data[saveKey] is T tValue)
                {
                    value = tValue;
                
                    return true;
                }
                
                break;
            }
            
            value = default;
            return false;
        }
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(SerializableGuid key, out T value)
        {
            foreach (var saveKey in Data.Keys)
            {
                if (saveKey != key) continue;

                if (Data[saveKey] is T tValue)
                {
                    value = tValue;
                
                    return true;
                }
                
                break;
            }

            value = default;

            return false;
        }
        
        /// <summary>
        /// Get value from Data by SerializableGuid
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="value">The value that will be found in Data or default value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>True if key is found, otherwise false</returns>
        public bool TryGetKey<T>(SaveKey key, out T value)
        {
            if (Data.TryGetValue(key, out var outValue) && outValue is T tValue)
            {
                value = tValue;
                return true;
            }

            value = default;
            return false;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Set value to Data by StringKey
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="value">Value to be set</param>
        /// <returns>This instance of SaveData</returns>
        public SaveData SetKey(string key, object value)
        {
            if (SerializableGuid.IsHexString(key))
                return SetKey(SerializableGuid.FromHexString(key), value);

            var foundedSaveKey = SaveKey.Empty;
            foreach (var saveKey in Data.Keys)
            {
                if (saveKey != key) continue;
                foundedSaveKey = saveKey;
                break;
            }

            if (foundedSaveKey.IsEmpty()) 
                foundedSaveKey = new SaveKey(key);
            
            Data[foundedSaveKey] = value;

            return this;
        }
        
        /// <summary>
        /// Set value to Data by StringKey
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="value">Value to be set</param>
        /// <returns>This instance of SaveData</returns>
        public SaveData SetKey(SerializableGuid key, object value)
        {
            var foundedSaveKey = SaveKey.Empty;
            
            foreach (var saveKey in Data.Keys)
            {
                if (saveKey != key) continue;
                
                foundedSaveKey = saveKey;
                break;
            }

            if (foundedSaveKey.IsEmpty()) 
                foundedSaveKey = new SaveKey(key);
            
            Data[foundedSaveKey] = value;

            return this;
        }
        
        /// <summary>
        /// Set value to Data by StringKey
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <param name="value">Value to be set</param>
        /// <returns>This instance of SaveData</returns>
        public SaveData SetKey(SaveKey key, object value)
        {
            Data[key] = value;

            return this;
        }

        #endregion

        #region Helpers
        
        /// <summary>
        /// Check if key exist in Data
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(string key)
        {
            if (SerializableGuid.IsHexString(key))
                return IsKeyExist(SerializableGuid.FromHexString(key));

            foreach (var k in Data.Keys)
                if (k.StringKey == key) return true;
            
            return false;
        }

        /// <summary>
        /// Check if key exist in Data
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(SerializableGuid key)
        {
            foreach (var k in Data.Keys)
                if (k.Key == key) return true;
            
            return false;
        }

        /// <summary>
        /// Check if key exist in Data
        /// </summary>
        /// <param name="key">Key for searching in Data</param>
        /// <returns>True if key exist, otherwise false</returns>
        public bool IsKeyExist(SaveKey key) => Data.ContainsKey(key);
        
        /// <summary>
        /// Remove key from Data
        /// </summary>
        /// <param name="key">Key to be removed</param>
        public void RemoveKey(string key)
        {
            if (SerializableGuid.IsHexString(key))
                RemoveKey(SerializableGuid.FromHexString(key));
            else
                foreach (var saveKey in Data.Keys)
                {
                    if (saveKey.StringKey != key) continue;
                    
                    Data.Remove(saveKey);
                    break;
                }
        }
        
        /// <summary>
        /// Remove key from Data
        /// </summary>
        /// <param name="key">Key to be removed</param>
        public void RemoveKey(SerializableGuid key)
        {
            foreach (var saveKey in Data.Keys)
            {
                if (saveKey.Key != key) continue;
                
                Data.Remove(saveKey);
                break;
            }
        }
        
        /// <summary>
        /// Remove key from Data
        /// </summary>
        /// <param name="key">Key to be removed</param>
        public void RemoveKey(SaveKey key) => Data.Remove(key);

        #endregion

        #region Save/Load
        
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

        /// <summary>
        /// The JsonConverter for BetterSaveData contains methods for writing and reading data
        /// </summary>
        internal class JsonConverter : JsonConverter<SaveData>
        {
            private int _depth;
            
            private readonly JsonSettings _jsonSettings;

            #region Constructors

            public JsonConverter(JsonSettings jsonSettings) => _jsonSettings = jsonSettings;

            #endregion

            public override void WriteJson(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                WriteStartFileComment(writer);
                
                if (writer.Formatting == Formatting.None) WriteJsonFormatingNone(writer, value, serializer);
                else WriteJsonFormatingIndented(writer, value, serializer);
            }

            private void WriteJsonFormatingNone(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    WriteJsonFormatingNoneBeforeObject(writer, value, serializer);
                else WriteJsonFormatingNoneAfterObject(writer, value, serializer);
            }

            private void WriteJsonFormatingIndented(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    WriteJsonFormatingIndentedBeforeObject(writer, value, serializer);
                else WriteJsonFormatingIndentedAfterObject(writer, value, serializer);
            }

            private static void WriteJsonFormatingNoneBeforeObject(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    WriteComment(writer, saveKey.Comment);
                    
                    writer.WritePropertyName(saveKey.ToString());
                    
                    if (saveValue is SaveData betterSaveData)
                        WriteJsonFormatingNoneBeforeObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                }

                writer.WriteEndObject();
            }

            private static void WriteJsonFormatingNoneAfterObject(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    writer.WritePropertyName(saveKey.ToString());
                    
                    if (saveValue is SaveData betterSaveData)
                        WriteJsonFormatingNoneAfterObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                    
                    WriteComment(writer, saveKey.Comment);
                }
                
                writer.WriteEndObject();
            }

            private void WriteJsonFormatingIndentedBeforeObject(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);

                _depth += 2;
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    WriteCommentIndentBeforeObject(writer, saveKey.Comment);
                    
                    writer.WritePropertyName(saveKey.ToString());

                    if (saveValue is SaveData betterSaveData)
                        WriteJsonFormatingIndentedBeforeObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                }
                
                writer.WriteEndObject();

                _depth -= 2;
            }
            
            private static void WriteJsonFormatingIndentedAfterObject(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    writer.WritePropertyName(saveKey.ToString());
                    
                    if (saveValue is SaveData betterSaveData)
                        WriteJsonFormatingIndentedAfterObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                    
                    WriteCommentIndentAfterObject(writer, saveKey.Comment);
                }
                
                writer.WriteEndObject();
            }
            
            private static void WriteIdentificationComment(JsonWriter writer)
            {
                if (!writer.Path.IsBlank()) 
                    writer.WriteComment("SaveData");
            }

            private void WriteStartFileComment(JsonWriter writer)
            {
                if (!_jsonSettings.HasFileComment || _jsonSettings.FileComment.IsBlank() ||
                    !writer.Path.IsBlank()) return;
                
                writer.WriteComment(_jsonSettings.FileComment);
                writer.WriteWhitespace("\n\n");
            }

            private void WriteCommentIndentBeforeObject(JsonWriter writer, HasValue<string> comment)
            {
                if (!comment.hasValue) return;
                
                var indentation = new string(' ', _depth);
                writer.WriteWhitespace($"\n{indentation}");
                writer.WriteComment(comment.value);
            }
            
            private static void WriteCommentIndentAfterObject(JsonWriter writer, HasValue<string> comment)
            {
                if (!comment.hasValue) return;

                writer.WriteWhitespace(" ");
                writer.WriteComment(comment.value);
            }

            private static void WriteComment(JsonWriter writer, HasValue<string> comment)
            {
                if (!comment.hasValue) return;

                writer.WriteComment(comment.value);
            }

            public override SaveData ReadJson(JsonReader reader, Type objectType, SaveData existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;

                while (reader.TokenType == JsonToken.Comment) 
                    reader.Read();

                var saveData = existingValue ?? new SaveData();
                
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    ReadJsonCommentBeforeObject(saveData, reader, serializer);
                else ReadJsonCommentAfterObject(saveData, reader, serializer);

                return saveData;
            }
            
            private void ReadJsonCommentBeforeObject(SaveData saveData, JsonReader reader, JsonSerializer serializer)
            {
                var jsonComment = string.Empty;

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            var jsonPropertyName = reader.Value?.ToString();

                            reader.Read();

                            var isBetterSaveData = IsBetterSaveData(reader);

                            var jsonPropertyValue = isBetterSaveData
                                ? serializer.Deserialize(reader, typeof(SaveData))
                                : serializer.Deserialize(reader);

                            var saveKey = new SaveKey(jsonPropertyName, jsonComment);
                            saveData.SetKey(saveKey, jsonPropertyValue);

                            jsonComment = string.Empty;
                            break;

                        case JsonToken.Comment:
                            jsonComment = reader.Value?.ToString();
                            break;
                    }
                }
            }

            private void ReadJsonCommentAfterObject(SaveData saveData, JsonReader reader, JsonSerializer serializer)
            {
                var readNextToken = true;
                while (true)
                {
                    if (readNextToken)
                    {
                        if (!reader.Read())
                            break;
                    }

                    readNextToken = true;

                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType != JsonToken.PropertyName)
                        continue;

                    var propertyName = reader.Value?.ToString();
                    reader.Read(); // Przesuń do wartości właściwości

                    // Deserializacja wartości
                    var isBetterSaveData = IsBetterSaveData(reader);
                    var propertyValue = isBetterSaveData
                        ? serializer.Deserialize(reader, typeof(SaveData))
                        : serializer.Deserialize(reader);

                    // Sprawdź czy po wartości występuje komentarz
                    var comment = "";
                    var hasReadAfterValue = reader.Read(); // Przesuń do tokena po wartości
        
                    if (hasReadAfterValue && reader.TokenType == JsonToken.Comment)
                    {
                        comment = reader.Value?.ToString();
                        readNextToken = true;
                    }
                    else
                    {
                        // Jeśli token nie jest komentarzem, nie przesuwaj czytnika dalej
                        readNextToken = false;
                    }

                    saveData.SetKey(new SaveKey(propertyName, comment), propertyValue);
                }
            }
            
            private static bool IsBetterSaveData(JsonReader reader)
            {
                if (reader.TokenType != JsonToken.Comment) return false;

                var comment = reader.Value?.ToString();

                if (string.IsNullOrEmpty(comment) || string.IsNullOrWhiteSpace(comment)) return false;

                var isBetterSaveData = comment.StartsWith("SaveData");

                if (isBetterSaveData) reader.Read();

                return isBetterSaveData;
            }
        }
    }
}