using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NoReleaseDate.Variables.HasValueVariable.Runtime;
using UnityEngine;

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

        [Serializable]
        internal class SaveFileComment
        {
            [SerializeField] private string _title;
            [SerializeField] private bool _writeGameVersion;
            [SerializeField] private bool _writeSaveTime;

            private HasValue<string> _customComment = new(string.Empty);
            
            internal string GetComment()
            {
                if (_customComment.hasValue)
                    return _customComment.value;
                
                var comment = string.Empty;
                
                if (!string.IsNullOrEmpty(_title) && !string.IsNullOrWhiteSpace(_title))
                    comment += "    " + _title + "\n";
                
                if (_writeGameVersion)
                    comment += "    Game version: " + Application.version + "\n";
                
                if (_writeSaveTime)
                    comment += "    Save time: " + DateTime.Now + "\n";

                if (!string.IsNullOrEmpty(comment) || !string.IsNullOrWhiteSpace(comment))
                    comment = "\n" + comment;
                
                return comment;
            }

            internal void SetCustomComment(string comment)
            {
                if (string.IsNullOrEmpty(comment) || string.IsNullOrWhiteSpace(comment))
                {
                    _customComment.hasValue = false;
                    return;
                }

                _customComment.hasValue = true;
                _customComment.value = comment;
            }
        }

        public bool HasFileComment
        {
            get => _fileComment.hasValue;
            set => _fileComment.hasValue = value;
        }

        public string FileComment
        {
            get => _fileComment.value.GetComment();
            set => _fileComment.value.SetCustomComment(value);
        }

        public JsonSerializerSettings JsonSerializerSettings => new() {
            TypeNameHandling = TypeNameHandling,
            Formatting = Formatting,
            Converters = Converters
        };

        private JsonConverter[] Converters => new JsonConverter[] {
            new SaveData.JsonConverter(this)
        };

        public TypeNameHandling TypeNameHandling = TypeNameHandling.None;
        public Formatting Formatting = Formatting.Indented;
        public CommentPosition WriteCommentPosition = CommentPosition.BeforeObject;

        [SerializeField] private HasValue<SaveFileComment> _fileComment = new(new SaveFileComment());
    }
}