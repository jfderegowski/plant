using System.Collections.Generic;
using System.Linq;
using NoReleaseDate.Variables.SerializableGuidVariable.Runtime;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    public class BaseData<T>
    {
        public Dictionary<SaveKey, T> Data { get; set; } = new();

        public TV GetKey<TV>(string key, TV defaultValue)
        {
            if (SerializableGuid.IsHexString(key))
                return GetKey(SerializableGuid.FromHexString(key), defaultValue);
            
            foreach (var saveKey in Data.Keys.Where(saveKey => saveKey.StringKey == key))
                if (Data[saveKey] is TV tVValue) 
                    return tVValue;
            
            return defaultValue;
        }
        
        public TV GetKey<TV>(SerializableGuid key, TV defaultValue)
        {
            foreach (var saveKey in Data.Keys.Where(saveKey => saveKey.Key == key))
                if (Data[saveKey] is TV tVValue) 
                    return tVValue;
            
            return defaultValue;
        }
        
        public TV GetKey<TV>(SaveKey key, TV defaultValue)
        {
            if (!Data.TryGetValue(key, out var saveValue)) return defaultValue;
            
            if (saveValue is TV tVValue) 
                return tVValue;
            
            return defaultValue;
        }

        public BaseData<T> SetKey(string key, T value)
        {
            if (SerializableGuid.IsHexString(key))
                return SetKey(SerializableGuid.FromHexString(key), value);

            var foundedSaveKey = SaveKey.Empty;
            foreach (var saveKey in Data.Keys.Where(saveKey => saveKey.StringKey == key))
            {
                foundedSaveKey = saveKey;
                break;
            }

            if (foundedSaveKey.IsEmpty()) 
                foundedSaveKey = new SaveKey(key);
            
            Data[foundedSaveKey] = value;

            return this;
        }
        
        public BaseData<T> SetKey(SerializableGuid key, T value)
        {
            var foundedSaveKey = SaveKey.Empty;
            foreach (var saveKey in Data.Keys.Where(saveKey => saveKey.Key == key))
            {
                foundedSaveKey = saveKey;
                break;
            }
            
            if (foundedSaveKey.IsEmpty()) 
                foundedSaveKey = new SaveKey(key);
            
            Data[foundedSaveKey] = value;

            return this;
        }
        
        public BaseData<T> SetKey(SaveKey key, T value)
        {
            Data[key] = value;

            return this;
        }

        public bool IsKeyExist(string key) =>
            !SerializableGuid.IsHexString(key)
                ? Data.Keys.Any(k => k.StringKey == key)
                : IsKeyExist(SerializableGuid.FromHexString(key));

        public bool IsKeyExist(SerializableGuid key) => Data.Keys.Any(k => k.Key == key);
        
        public bool IsKeyExist(SaveKey key) => Data.ContainsKey(key);
        
        public void RemoveKey(string key)
        {
            if (!SerializableGuid.IsHexString(key))
                foreach (var saveKey in Data.Keys.Where(saveKey => saveKey.StringKey == key))
                {
                    Data.Remove(saveKey);
                    return;
                }
            else RemoveKey(SerializableGuid.FromHexString(key));
        }
        
        public void RemoveKey(SerializableGuid key)
        {
            foreach (var saveKey in Data.Keys.Where(saveKey => saveKey.Key == key))
            {
                Data.Remove(saveKey);
                return;
            }
        }
        
        public void RemoveKey(SaveKey key) => Data.Remove(key);
        
        public void ClearData() => Data.Clear();
    }
}