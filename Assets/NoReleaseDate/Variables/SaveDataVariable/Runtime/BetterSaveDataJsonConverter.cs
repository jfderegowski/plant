using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NoReleaseDate.Variables.HasValueVariable.Runtime;
using NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    public class BetterSaveDataJsonConverter : JsonConverter<BetterSaveData>
    {
        private readonly JsonSettings _jsonSettings;
        
        public BetterSaveDataJsonConverter() => _jsonSettings = new JsonSettings();

        public BetterSaveDataJsonConverter(JsonSettings jsonSettings) => _jsonSettings = jsonSettings;

        public override void WriteJson(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
        {
            var writeSaveDataId = !string.IsNullOrEmpty(writer.Path);
            
            if (writeSaveDataId)
                writer.WriteComment("SaveData");

            writer.WriteStartObject();

            foreach (var (saveKey, saveValue) in value.Data)
            {
                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject)
                    WriteComment(writer, saveKey.Comment);

                writer.WritePropertyName(saveKey.ToString());
                serializer.Serialize(writer, saveValue);

                if (_jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.AfterObject)
                    WriteComment(writer, saveKey.Comment);
            }

            writer.WriteEndObject();
        }

        private void WriteComment(JsonWriter writer, HasValue<string> comment)
        {
            if (!comment.hasValue) return;
            
            var isPrettyPrint = writer.Formatting == Formatting.Indented;

            var depth = !string.IsNullOrEmpty(writer.Path) ? writer.Path.Split(".").Length + 1 : 1;
            
            var indentation = isPrettyPrint ? new string(' ', depth * 2) : string.Empty;
            
            if (isPrettyPrint)
                writer.WriteWhitespace(
                    _jsonSettings.WriteCommentPosition == JsonSettings.CommentPosition.BeforeObject
                        ? $"\n{indentation}"
                        : " "
                );

            writer.WriteComment(comment.value);
        }

        private void WriteCommentPrettyPrintBeforeObject(JsonWriter writer, HasValue<string> comment)
        {
            var depth = !string.IsNullOrEmpty(writer.Path) ? writer.Path.Split(".").Length + 1 : 1;
            
            var indentation = new string(' ', depth * 2);

            writer.WriteWhitespace($"\n{indentation}");
            
            writer.WriteComment(comment.value);
        }
        
        private void WriteCommentPrettyPrintAfterObject(JsonWriter writer, HasValue<string> comment)
        {
            writer.WriteWhitespace(" ");
            
            writer.WriteComment(comment.value);
        }
        
        private void WriteCommentBeforeObject(JsonWriter writer, HasValue<string> comment)
        {
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