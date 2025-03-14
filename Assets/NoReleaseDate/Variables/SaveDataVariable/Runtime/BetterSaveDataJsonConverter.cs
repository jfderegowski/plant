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
        public BetterSaveDataJsonConverter()
        {
            _jsonSettings = new JsonSettings();
        }
        
        public BetterSaveDataJsonConverter(JsonSettings jsonSettings) => _jsonSettings = jsonSettings;

        private static JsonSettings _jsonSettings;
        private int _indentLevel;

        public override void WriteJson(JsonWriter writer, BetterSaveData value, JsonSerializer serializer)
        {
            _indentLevel++;
            
            writer.WriteComment("BETTERSAVEDATA");
            writer.WriteStartObject();

            var typeName = GetTypeName(value.GetType(), serializer.TypeNameAssemblyFormatHandling, serializer.SerializationBinder);
            writer.WritePropertyName("$type");
            writer.WriteValue(typeName);

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

            // while (reader.Read())
            // {
            //     Debug.Log($"{reader.Value}");
            // }
            //
            // return new BetterSaveData();
            
            Debug.Log("=====START=====");
            Debug.Log($"=TokenType: {reader.TokenType}: {reader.Value}=");

            if (reader.TokenType == JsonToken.Comment) reader.Read();
            
            if (reader.TokenType != JsonToken.StartObject)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                
                throw new JsonSerializationException("Expected StartObject token.");
            }

            var saveData = existingValue ?? new BetterSaveData();

            var jsonComment = string.Empty;
            var isBetterSaveData = false;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                Debug.Log($"{reader.TokenType}: {reader.Value}");

                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var jsonPropertyName = reader.Value?.ToString();

                        if (jsonPropertyName == "$type") continue;
                        
                        reader.Read();
                        
                        if (reader.TokenType == JsonToken.Comment)
                        {
                            if (reader.Value?.ToString() == "BETTERSAVEDATA") 
                                isBetterSaveData = true;
                            
                            reader.Read();
                        }
                        
                        Debug.Log($"{reader.TokenType}: {reader.Value}");

                        var jsonPropertyValue = isBetterSaveData
                            ? serializer.Deserialize(reader, typeof(BetterSaveData))
                            : serializer.Deserialize(reader);

                        if (isBetterSaveData) isBetterSaveData = false;
                        
                        var saveKey = new SaveKey(jsonPropertyName, jsonComment);
                        saveData.Set(saveKey, jsonPropertyValue);

                        jsonComment = string.Empty;
                        break;

                    case JsonToken.Comment:
                        jsonComment = reader.Value?.ToString();
                        break;
                }
            }

            Debug.Log($"=====END=====");
            
            return saveData;
        }

        private BetterSaveData ReadJsonWithCommentBeforeObject(BetterSaveData saveData, JsonReader reader,
            JsonSerializer serializer)
        {
            var jsonComment = string.Empty;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                Debug.Log($"{reader.TokenType}: {reader.Value}");

                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var jsonPropertyName = reader.Value?.ToString();

                        if (jsonPropertyName == "$type") continue;
                        
                        reader.Read();
                        
                        Debug.Log($"{reader.TokenType}: {reader.Value}");
                        
                        var jsonPropertyValue = serializer.Deserialize(reader);
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
        
        private BetterSaveData ReadJsonWithCommentAfterObject(BetterSaveData saveData, JsonReader reader, JsonSerializer serializer)
        {
            // TODO: Implement this method later
            return null;
        }
        
        public static string GetTypeName(Type t, TypeNameAssemblyFormatHandling assemblyFormat, ISerializationBinder? binder)
        {
            string fullyQualifiedTypeName = GetFullyQualifiedTypeName(t, binder);

            switch (assemblyFormat)
            {
                case TypeNameAssemblyFormatHandling.Simple:
                    return RemoveAssemblyDetails(fullyQualifiedTypeName);
                case TypeNameAssemblyFormatHandling.Full:
                    return fullyQualifiedTypeName;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static string GetFullyQualifiedTypeName(Type t, ISerializationBinder? binder)
        {
            if (binder != null)
            {
                binder.BindToName(t, out string? assemblyName, out string? typeName);
#if (NET20 || NET35)
                // for older SerializationBinder implementations that didn't have BindToName
                if (assemblyName == null & typeName == null)
                {
                    return t.AssemblyQualifiedName;
                }
#endif
                return typeName + (assemblyName == null ? "" : ", " + assemblyName);
            }

            return t.AssemblyQualifiedName!;
        }
        
        private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
        {
            StringBuilder builder = new StringBuilder();

            // loop through the type name and filter out qualified assembly details from nested type names
            bool writingAssemblyName = false;
            bool skippingAssemblyDetails = false;
            bool followBrackets = false;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char current = fullyQualifiedTypeName[i];
                switch (current)
                {
                    case '[':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        followBrackets = true;
                        builder.Append(current);
                        break;
                    case ']':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        followBrackets = false;
                        builder.Append(current);
                        break;
                    case ',':
                        if (followBrackets)
                        {
                            builder.Append(current);
                        }
                        else if (!writingAssemblyName)
                        {
                            writingAssemblyName = true;
                            builder.Append(current);
                        }
                        else
                        {
                            skippingAssemblyDetails = true;
                        }
                        break;
                    default:
                        followBrackets = false;
                        if (!skippingAssemblyDetails)
                        {
                            builder.Append(current);
                        }
                        break;
                }
            }

            return builder.ToString();
        }
    }
}