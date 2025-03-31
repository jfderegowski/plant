using System;
using fefek5.Common.Runtime.Extensions;
using fefek5.Variables.HasValueVariable.Runtime;
using fefek5.Variables.SerializableGuidVariable.Runtime;
using UnityEngine;

namespace fefek5.Variables.SaveDataVariable.Runtime
{
    [Serializable]
    public struct SaveKey : IEquatable<SaveKey>, IEquatable<SerializableGuid>, IEquatable<string>
    {
        #region Fields

        [Tooltip("Use this key to have fast compare")] 
        public SerializableGuid Key;

        [Tooltip("If the value is not empty, it will be used instead of the SerializableGuid key.")]
        public HasValue<string> StringKey;
        
        [Tooltip("Comment for the key. If its empty it will no serialize in file")] 
        public HasValue<string> Comment;

        #endregion

        #region Properties

        public static SaveKey RandomKey => new(SerializableGuid.NewGuid());
        
        public static SaveKey Empty => new(SerializableGuid.Empty);

        #endregion

        #region Constructors

        public SaveKey(SerializableGuid key)
        {
            Key = key;
            StringKey = new HasValue<string>(string.Empty, false);
            Comment = new HasValue<string>(string.Empty, false);
        }
        
        public SaveKey(SerializableGuid key, string comment)
        {
            Key = key;
            StringKey = new HasValue<string>(string.Empty, false);
            Comment = new HasValue<string>(comment, !comment.IsBlank());
        }
        
        public SaveKey(string key)
        {
            // if (key.IsBlank())
            //     throw new ArgumentException("Key is empty. Use Empty instead.");
            
            if (SerializableGuid.IsHexString(key))
            {
                Key = SerializableGuid.FromHexString(key);
                StringKey = new HasValue<string>(string.Empty, false);
            }
            else
            {
                Key = SerializableGuid.Empty;
                StringKey = new HasValue<string>(key, true);
            }
            
            Comment = new HasValue<string>(string.Empty, false);
        }
        
        public SaveKey(string key, string comment)
        {
            if (SerializableGuid.IsHexString(key))
            {
                Key = SerializableGuid.FromHexString(key);
                StringKey = new HasValue<string>(string.Empty, false);
            }
            else
            {
                Key = SerializableGuid.Empty;
                StringKey = new HasValue<string>(key, true);
            }
            
            Comment = new HasValue<string>(comment, !comment.IsBlank());
        }

        public SaveKey(SaveKey saveKey)
        {
            Key = saveKey.Key;
            StringKey = saveKey.StringKey;
            Comment = saveKey.Comment;
        }

        #endregion
        
        #region Methods
        
        public SaveKey SetKey(string key)
        {
            if (key.IsBlank())
            {
                Debug.LogWarning("Key is empty. SetKey method will not change the key.");
                return this;
            }
            
            if (SerializableGuid.IsHexString(key))
                return SetKey(SerializableGuid.FromHexString(key));

            StringKey.Set(key, true);
            Key = SerializableGuid.Empty;

            return this;
        }
        
        public SaveKey SetKey(SerializableGuid key)
        {
            Key = key;
            StringKey.Set(string.Empty, false);
            return this;
        }

        public SaveKey SetComment(string comment)
        {
            Comment.Set(comment, !comment.IsBlank());
            return this;
        }
        
        public SaveKey RemoveComment()
        {
            Comment.Set(string.Empty, false);
            return this;
        }

        #endregion

        #region Conditionals

        public bool IsEmpty() => StringKey.hasValue ? StringKey.value.IsBlank() : Key.IsEmpty;

        public bool IsValid() => !IsEmpty();

        #endregion

        #region Equality

        public bool Equals(SaveKey other)
        {
            if (StringKey.hasValue && other.StringKey.hasValue)
                return StringKey.value == other.StringKey.value;
            
            return Key == other.Key; 
        }
        
        public bool Equals(SerializableGuid other) => !StringKey.hasValue && Key == other;

        public bool Equals(string other)
        {
            if (SerializableGuid.IsHexString(other)) 
                return Equals(SerializableGuid.FromHexString(other));
            
            return StringKey.hasValue && StringKey.value == other;
        }

        public override bool Equals(object obj) => obj switch {
            null => false,
            SaveKey other => Equals(other),
            SerializableGuid serializableGuid => Equals(serializableGuid),
            string str => Equals(str),
            _ => false
        };

        #endregion
        
        #region GetHashCode

        public override int GetHashCode() => StringKey.hasValue 
            ? HashCode.Combine(true, StringKey.value) 
            : HashCode.Combine(false, Key);

        #endregion
        
        #region Operators

        public static bool operator ==(SaveKey left, SaveKey right) => left.Equals(right);

        public static bool operator !=(SaveKey left, SaveKey right) => !left.Equals(right);
        
        public static bool operator ==(SaveKey left, SerializableGuid right) => left.Equals(right);
        
        public static bool operator !=(SaveKey left, SerializableGuid right) => !left.Equals(right);
        
        public static bool operator ==(SerializableGuid left, SaveKey right) => right.Equals(left);
        
        public static bool operator !=(SerializableGuid left, SaveKey right) => !right.Equals(left);
        
        public static bool operator ==(SaveKey left, string right) => left.Equals(right);
        
        public static bool operator !=(SaveKey left, string right) => !left.Equals(right);
        
        public static bool operator ==(string left, SaveKey right) => right.Equals(left);
        
        public static bool operator !=(string left, SaveKey right) => !right.Equals(left);

        #endregion

        #region Implicit
        
        public static implicit operator SaveKey(SerializableGuid key) => new(key);
        
        public static implicit operator SaveKey(string key) => new(key);
        
        public static implicit operator SerializableGuid(SaveKey saveKey) => 
            saveKey.StringKey.hasValue ? SerializableGuid.Empty : saveKey.Key;

        public static implicit operator string(SaveKey saveKey) =>
            saveKey.StringKey.hasValue ? saveKey.StringKey.value : saveKey.Key.ToHexString();
        
        #endregion

        #region ToString

        public override string ToString() => 
            StringKey.hasValue ? StringKey.value : Key.ToString();

        #endregion
    }
}
