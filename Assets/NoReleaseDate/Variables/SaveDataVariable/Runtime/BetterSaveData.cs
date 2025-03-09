using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoReleaseDate.Common.Runtime.Extensions;
using NoReleaseDate.Systems.EncryptionSystem;
using NoReleaseDate.Variables.HasValueVariable.Runtime;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using NoReleaseDate.Variables.SerializableGuidVariable.Runtime;
using NUnit.Framework.Internal.Execution;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    [Serializable]
    public class BetterSaveData
    {
        public Dictionary<SaveKey, object> Data { get; private set; } = new();

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

        [JsonConverter(typeof(JsonConverter))]
        internal class JsonConverter : JsonConverter<BetterSaveData>
        {
            public JsonConverter(JsonSettings jsonSettings) => _jsonSettings = jsonSettings;

            private static JsonSettings _jsonSettings;
            private int _indentLevel;

            public override void WriteJson(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                _indentLevel++;
                
                writer.WriteStartObject();

                var type = value.GetType();
                var assemblyName = type.Assembly.GetName().Name;
                writer.WritePropertyName("$type");
                writer.WriteValue($"{type.FullName}, {assemblyName}");

                var isPrettyPrint = writer.Formatting == Formatting.Indented;
                var indentation = isPrettyPrint ? new string(' ', _indentLevel * 2) : string.Empty;

                foreach (var (saveKey, saveValue) in value.Data)
                {
                    if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                        WriteCommentWithFormatting(writer, saveKey.Comment, isPrettyPrint, indentation);

                    writer.WritePropertyName(saveKey.ToString());
                    serializer.Serialize(writer, saveValue);

                    if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.AfterObject)
                        WriteCommentWithFormatting(writer, saveKey.Comment, isPrettyPrint, indentation);
                }

                writer.WriteEndObject();

                _indentLevel--;
            }

            private static void WriteCommentWithFormatting(JsonWriter writer, HasValue<string> comment,
                bool isPrettyPrint, string indentation)
            {
                if (!comment.hasValue) return;

                if (isPrettyPrint)
                    writer.WriteWhitespace(
                        _jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject
                            ? $"\n{indentation}"
                            : " "
                    );
                
                writer.WriteComment(comment.value);
            }

            public override BetterSaveData ReadJson(JsonReader reader, Type objectType, BetterSaveData existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                var saveData = existingValue ?? new BetterSaveData();
                string currentComment = null;

                string jsonComment = null;
                string jsonName = null;
                object jsonValue = null;
                
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.Comment:
                            currentComment = reader.Value?.ToString();
                            break;

                        case JsonToken.PropertyName:
                            var propertyName = reader.Value.ToString();
                            
                            if (propertyName == "$type")
                            {
                                reader.Read(); // Przejdź do wartości
                                break;
                            }
                            
                            reader.Read(); // Przejdź do wartości

                            var key = ParseSaveKey(propertyName);
                            if (!string.IsNullOrEmpty(currentComment))
                            {
                                key = key.WithComment(currentComment);
                                currentComment = null; // Zresetuj komentarz po przypisaniu
                            }

                            var value = DeserializeValue(reader, serializer);
                            saveData.Set(key, value);
                            break;
                    }
                }

                return saveData;
            }

            private static object DeserializeValue(JsonReader reader, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartObject) return serializer.Deserialize(reader);
                
                var nestedData = new BetterSaveData();
                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != JsonToken.PropertyName) continue;
                        
                    var key = ParseSaveKey(reader.Value.ToString());
                    
                    reader.Read();
                    
                    nestedData.Set(key, DeserializeValue(reader, serializer));
                }
                
                return nestedData;
            }

            private static SaveKey ParseSaveKey(string keyString) =>
                SerializableGuid.IsHexString(keyString)
                    ? new SaveKey(SerializableGuid.FromHexString(keyString))
                    : new SaveKey(keyString);
        }
    }
}