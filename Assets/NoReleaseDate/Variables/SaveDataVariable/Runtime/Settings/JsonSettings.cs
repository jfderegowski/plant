using System;
using Newtonsoft.Json;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime.Settings
{
    [Serializable]
    public class JsonSettings
    {
        public enum CommentPosition
        {
            BeforeObject,
            AfterObject
        }

        private JsonConverter[] Converters => new JsonConverter[] {
            new SaveData.JsonConverter(),
            new BetterSaveData.JsonConverter(this)
        };

        public JsonSerializerSettings JsonSerializerSettings => new() {
            TypeNameHandling = TypeNameHandling,
            Formatting = Formatting,
            Converters = Converters
        };

        public TypeNameHandling TypeNameHandling = TypeNameHandling.All;
        public Formatting Formatting = Formatting.Indented;
        public CommentPosition WriteCommentPosition = CommentPosition.BeforeObject;
    }
}