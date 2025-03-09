using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NoReleaseDate.Common.Runtime.Extensions;
using NoReleaseDate.Systems.EncryptionSystem;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using NoReleaseDate.Variables.SerializableGuidVariable.Runtime;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    [Serializable]
    public class SaveData : BaseData<ClassData>
    {
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
            JsonConvert.SerializeObject(new SaveData { Data = Data }, jsonSerializerSettings);

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

        internal class JsonConverter : JsonConverter<SaveData>
        {
            public override void WriteJson(JsonWriter writer, SaveData value, JsonSerializer serializer)
            {
                writer.WriteComment($" Save at time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ");
                writer.WriteWhitespace("\n\n");

                writer.WriteStartObject();

                foreach (var saveData in value.Data)
                {
                    if (saveData.Key.Comment.hasValue)
                    {
                        writer.WriteWhitespace("\n  ");
                        writer.WriteComment(saveData.Key.Comment);
                    }

                    writer.WritePropertyName(saveData.Key.StringKey.hasValue
                        ? saveData.Key.StringKey
                        : saveData.Key.Key.ToHexString());

                    writer.WriteStartObject();

                    foreach (var clasData in saveData.Value.Data)
                    {
                        if (clasData.Key.Comment.hasValue)
                        {
                            writer.WriteWhitespace("\n    ");
                            writer.WriteComment(clasData.Key.Comment);
                        }

                        writer.WritePropertyName(clasData.Key.StringKey.hasValue
                            ? clasData.Key.StringKey
                            : clasData.Key.Key.ToHexString());

                        serializer.Serialize(writer, clasData.Value);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            public override SaveData ReadJson(JsonReader reader, Type objectType, SaveData existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                ValidateObjectType(objectType);
                SkipInitialCommentsAndWhitespace(reader);
                ValidateStartObject(reader);

                var saveData = new SaveData();
                ProcessSaveDataProperties(reader, serializer, saveData);

                return saveData;
            }

            private static void ValidateObjectType(Type objectType)
            {
                if (objectType != typeof(SaveData))
                    throw new ArgumentException("Expected SaveData type");
            }

            private static void SkipInitialCommentsAndWhitespace(JsonReader reader)
            {
                reader.Read();
                while (reader.TokenType == JsonToken.Comment)
                    reader.Read();
            }

            private static void ValidateStartObject(JsonReader reader)
            {
                if (reader.TokenType != JsonToken.StartObject)
                    throw new JsonException("Expected start of object");

                reader.Read();
            }

            private static void ProcessSaveDataProperties(JsonReader reader, JsonSerializer serializer, SaveData saveData)
            {
                while (reader.TokenType != JsonToken.EndObject)
                {
                    var (propertyComment, propertyName) = ReadPropertyWithComment(reader);
                    var saveKey = CreateSaveKey(propertyName, propertyComment);
                    var classData = ProcessClassDataObject(reader, serializer);

                    saveData.SetKey(saveKey, classData);
                    reader.Read();
                }
            }

            private static (string comment, string propertyName) ReadPropertyWithComment(JsonReader reader)
            {
                var comment = string.Empty;
                if (reader.TokenType == JsonToken.Comment)
                {
                    comment = (string)reader.Value;
                    reader.Read();
                }

                ValidateTokenType(reader, JsonToken.PropertyName);
                var propertyName = (string)reader.Value;
                reader.Read(); // Move to value

                return (comment, propertyName);
            }

            private static SaveKey CreateSaveKey(string keyValue, string comment) =>
                keyValue.Length == 32
                    ? new SaveKey(SerializableGuid.FromHexString(keyValue), comment)
                    : new SaveKey(keyValue, comment);

            private static ClassData ProcessClassDataObject(JsonReader reader, JsonSerializer serializer)
            {
                ValidateTokenType(reader, JsonToken.StartObject);
                reader.Read();

                var classData = new ClassData();
                ProcessClassDataProperties(reader, serializer, classData);

                return classData;
            }

            private static void ProcessClassDataProperties(JsonReader reader, JsonSerializer serializer, ClassData classData)
            {
                while (reader.TokenType != JsonToken.EndObject)
                {
                    var (propertyComment, propertyName) = ReadPropertyWithComment(reader);
                    var saveKey = CreateSaveKey(propertyName, propertyComment);
                    var value = DeserializeValue(reader, serializer);

                    classData.SetKey(saveKey, value);
                    reader.Read(); // Move to next property
                }
            }

            private static object DeserializeValue(JsonReader reader, JsonSerializer serializer) =>
                reader.TokenType != JsonToken.Null ? serializer.Deserialize(reader) : null;

            private static void ValidateTokenType(JsonReader reader, JsonToken expectedToken)
            {
                if (reader.TokenType != expectedToken)
                    throw new JsonException($"Expected {expectedToken}");
            }
        }
    }
}