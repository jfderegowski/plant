using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NoReleaseDate.Common.Runtime.Extensions;
using NoReleaseDate.Systems.EncryptionSystem;
using NoReleaseDate.Variables.HasValueVariable.Runtime;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    [Serializable]
    public class BetterSaveData
    {
        public Dictionary<SaveKey, object> Data { get; private set; }

        #region Constructors

        public BetterSaveData() => Data = new Dictionary<SaveKey, object>();

        public BetterSaveData(Dictionary<SaveKey, object> data) => Data = new Dictionary<SaveKey, object>(data);

        public BetterSaveData(BetterSaveData saveData) => Data = new Dictionary<SaveKey, object>(saveData.Data);

        #endregion

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

        #region Save/Load

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

                    Debug.Log($"[SAVE-DATA] Saved to file: {path} "
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

        #endregion

        #region ToJson

        public string ToJson() => ToJson(SaveSettings.Default);

        public string ToJson(SaveSettings saveSettings) =>
            saveSettings.UseJsonCustomSettings
                ? ToJson(saveSettings.JsonCustomSettings)
                : ToJson(new JsonSerializerSettings());

        public string ToJson(JsonSettings jsonSettings) => ToJson(jsonSettings.JsonSerializerSettings);

        public string ToJson(JsonSerializerSettings jsonSerializerSettings) =>
            JsonConvert.SerializeObject(new BetterSaveData(this), jsonSerializerSettings);

        #endregion

        #region File Management

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

        #endregion

        internal class JsonConverter : JsonConverter<BetterSaveData>
        {
            private int _depth;
            
            private readonly JsonSettings _jsonSettings;

            #region Constructors

            public JsonConverter(JsonSettings jsonSettings) => _jsonSettings = jsonSettings;

            #endregion

            public override void WriteJson(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                if (writer.Formatting == Formatting.None) WriteJsonFormatingNone(writer, value, serializer);
                else WriteJsonFormatingIndented(writer, value, serializer);
            }

            private void WriteJsonFormatingNone(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    WriteJsonFormatingNoneBeforeObject(writer, value, serializer);
                else WriteJsonFormatingNoneAfterObject(writer, value, serializer);
            }

            private void WriteJsonFormatingIndented(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    WriteJsonFormatingIndentedBeforeObject(writer, value, serializer);
                else WriteJsonFormatingIndentedAfterObject(writer, value, serializer);
            }

            private static void WriteJsonFormatingNoneBeforeObject(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    WriteComment(writer, saveKey.Comment);
                    
                    writer.WritePropertyName(saveKey.ToString());
                    
                    if (saveValue is BetterSaveData betterSaveData)
                        WriteJsonFormatingNoneBeforeObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                }

                writer.WriteEndObject();
            }

            private static void WriteJsonFormatingNoneAfterObject(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    writer.WritePropertyName(saveKey.ToString());
                    
                    if (saveValue is BetterSaveData betterSaveData)
                        WriteJsonFormatingNoneAfterObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                    
                    WriteComment(writer, saveKey.Comment);
                }
                
                writer.WriteEndObject();
            }

            private void WriteJsonFormatingIndentedBeforeObject(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);

                _depth += 2;
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    WriteCommentIndentBeforeObject(writer, saveKey.Comment);
                    
                    writer.WritePropertyName(saveKey.ToString());

                    if (saveValue is BetterSaveData betterSaveData)
                        WriteJsonFormatingIndentedBeforeObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                }
                
                writer.WriteEndObject();

                _depth -= 2;
            }
            
            private static void WriteJsonFormatingIndentedAfterObject(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
            {
                WriteIdentificationComment(writer);
                
                writer.WriteStartObject();
                
                foreach (var (saveKey, saveValue) in value.Data)
                {
                    writer.WritePropertyName(saveKey.ToString());
                    
                    if (saveValue is BetterSaveData betterSaveData)
                        WriteJsonFormatingIndentedAfterObject(writer, betterSaveData, serializer);
                    else serializer.Serialize(writer, saveValue);
                    
                    WriteCommentIndentAfterObject(writer, saveKey.Comment);
                }
                
                writer.WriteEndObject();
            }
            
            private static void WriteIdentificationComment(JsonWriter writer)
            {
                if (!string.IsNullOrWhiteSpace(writer.Path)) 
                    writer.WriteComment("SaveData");
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

            public override BetterSaveData ReadJson(JsonReader reader, Type objectType, BetterSaveData existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Null:
                        return null;
                    case JsonToken.Comment:
                        reader.Read();
                        break;
                }

                var saveData = existingValue ?? new BetterSaveData();
                
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    ReadJsonCommentBeforeObject(saveData, reader, serializer);
                else ReadJsonCommentAfterObject(saveData, reader, serializer);

                return saveData;
            }
            
            private BetterSaveData ReadJsonCommentBeforeObject(BetterSaveData saveData, JsonReader reader, JsonSerializer serializer)
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
                                ? serializer.Deserialize(reader, typeof(BetterSaveData))
                                : serializer.Deserialize(reader);

                            var saveKey = new SaveKey(jsonPropertyName, jsonComment);
                            saveData.Set(saveKey, jsonPropertyValue);

                            jsonComment = string.Empty;
                            break;

                        case JsonToken.Comment:
                            jsonComment = reader.Value?.ToString();
                            break;
                    }
                }
                
                return saveData;
            }

            private BetterSaveData ReadJsonCommentAfterObject(BetterSaveData saveData, JsonReader reader, JsonSerializer serializer)
            {
                bool readNextToken = true;
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

                    string propertyName = reader.Value?.ToString();
                    reader.Read(); // Przesuń do wartości właściwości

                    // Deserializacja wartości
                    bool isBetterSaveData = IsBetterSaveData(reader);
                    object propertyValue = isBetterSaveData
                        ? serializer.Deserialize(reader, typeof(BetterSaveData))
                        : serializer.Deserialize(reader);

                    // Sprawdź czy po wartości występuje komentarz
                    string comment = "";
                    bool hasReadAfterValue = reader.Read(); // Przesuń do tokena po wartości
        
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

                    saveData.Set(new SaveKey(propertyName, comment), propertyValue);
                }
    
                return saveData;
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