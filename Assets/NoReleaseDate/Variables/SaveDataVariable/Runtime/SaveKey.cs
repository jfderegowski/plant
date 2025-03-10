﻿using System;
using NoReleaseDate.Common.Runtime.Extensions;
using NoReleaseDate.Variables.HasValueVariable.Runtime;
using NoReleaseDate.Variables.SerializableGuidVariable.Runtime;
using UnityEngine;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    [Serializable]
    public struct SaveKey : IEquatable<SaveKey>
    {
        [Tooltip("Use this key to have fast compare")] 
        public SerializableGuid Key;
        
        [Tooltip("If the value is not empty, it will be used instead of the SerializableGuid key.")]
        public HasValue<string> StringKey;
        
        [Tooltip("Comment for the key. If its empty it will no serialize in file")] 
        public HasValue<string> Comment;
        
        public static SaveKey RandomKey => new(SerializableGuid.NewGuid());
        
        public static SaveKey Empty => new(SerializableGuid.Empty);

        public SaveKey(SerializableGuid key, string comment = "")
        {
            Key = key;
            StringKey = new HasValue<string>(string.Empty);
            Comment = new HasValue<string>(comment, !string.IsNullOrEmpty(comment));
        }
        
        public SaveKey(string key, string comment = "")
        {
            Key = SerializableGuid.Empty;
            StringKey = new HasValue<string>(key, true);
            Comment = new HasValue<string>(comment, !string.IsNullOrEmpty(comment));
        }

        public SaveKey(SaveKey saveKey)
        {
            Key = saveKey.Key;
            StringKey = saveKey.StringKey;
            Comment = saveKey.Comment;
        }

        public SaveKey WithKey(SerializableGuid key)
        {
            Key = key;
            return this;
        }

        public SaveKey WithComment(string comment)
        {
            Comment = new HasValue<string>(comment, !string.IsNullOrEmpty(comment));
            return this;
        }

        public bool IsEmpty() => Key.IsEmpty || (StringKey.hasValue && string.IsNullOrEmpty(StringKey));

        public bool IsValid() => !Key.IsEmpty || (StringKey.hasValue && !string.IsNullOrEmpty(StringKey));

        public bool Equals(SaveKey other)
        {
            if (StringKey.hasValue && other.StringKey.hasValue)
                return StringKey == other.StringKey;
            
            return Key == other.Key; 
        }
        
        public bool Equals(SerializableGuid other) => Key == other;
        
        public bool Equals(string other) => StringKey == other;

        public override bool Equals(object obj) => obj is SaveKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Key);

        public static bool operator ==(SaveKey left, SaveKey right) => left.Equals(right);

        public static bool operator !=(SaveKey left, SaveKey right) => !(left == right);

        public override string ToString()
        {
            if (StringKey.hasValue && !StringKey.value.IsBlank())
                return StringKey.value;

            return Key.ToString();
        }
    }
}
