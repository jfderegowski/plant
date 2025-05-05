using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fefek5.Variables.SaveDataVariable.Runtime
{
    internal static class Extensions
    {
        /// <summary>
        /// Get the value of a dictionary by key and convert it to the specified type.
        /// If the key does not exist or the conversion fails, return the default value.
        /// </summary>
        /// <param name="dict">The dictionary to search.</param>
        /// <param name="key">The key to look for.</param>
        /// <param name="defaultValue">The default value to return if the key does not exist or conversion fails.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <typeparam name="TK">The type of the key in the dictionary.</typeparam>
        /// <typeparam name="TV">The type of the value in the dictionary.</typeparam>
        /// <returns>The value converted to the specified type, or the default value if the conversion fails.</returns>
        public static T GetAs<T, TK, TV>(this IDictionary<TK, TV> dict, TK key, T defaultValue = default)
        {
            if (!dict.TryGetValue(key, out var rawValue) || rawValue is null)
                return defaultValue;

            if (rawValue is T t)
                return t;

            var targetType = typeof(T);

            try
            {
                if (rawValue is JToken jToken)
                    return jToken.ToObject<T>();

                if (targetType.IsEnum && rawValue is IConvertible)
                    return (T)Enum.ToObject(targetType, rawValue);

                if (rawValue is IConvertible)
                    return (T)Convert.ChangeType(rawValue, targetType);
            }
            catch
            {
                // Conversion failed; fall through to return defaultValue
            }

            return defaultValue;
        }

        #region Json Write Type

        /// <summary>
        /// Writes the type of the object to the JSON writer.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> used for serialization.</param>
        /// <param name="value">The object whose type to write.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>True if the type was written; otherwise, false.</returns>
        public static bool TryWriteType<T>(this JsonWriter writer, JsonSerializer serializer, T value) => 
            writer.TryWriteType(serializer, typeof(T));

        /// <summary>
        /// Writes the type of the object to the JSON writer if TypeNameHandling is not None.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> used for serialization.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>True if the type was written; otherwise, false.</returns>
        public static bool TryWriteType<T>(this JsonWriter writer, JsonSerializer serializer) => 
            writer.TryWriteType(serializer, typeof(T));

        /// <summary>
        /// Writes the type of the object to the JSON writer if TypeNameHandling is not None.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> used for serialization.</param>
        /// <param name="type">The type of the object.</param>
        /// <returns>True if the type was written; otherwise, false.</returns>
        public static bool TryWriteType(this JsonWriter writer, JsonSerializer serializer, Type type)
        {
            if (serializer.TypeNameHandling == TypeNameHandling.None)
                return false;

            writer.WriteType(type);
            
            return true;
        }

        /// <summary>
        /// Writes the type of the object to the JSON writer.
        /// </summary>
        /// <param name="writer"><see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The object whose type to write.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        public static void WriteType<T>(this JsonWriter writer, T value) => writer.WriteType(typeof(T));

        /// <summary>
        /// Writes the type of the object to the JSON writer.
        /// </summary>
        /// <param name="writer"><see cref="JsonWriter"/> to write to.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        public static void WriteType<T>(this JsonWriter writer) => writer.WriteType(typeof(T));

        /// <summary>
        /// Writes the type of the object to the JSON writer.
        /// </summary>
        /// <param name="writer"><see cref="JsonWriter"/> to write to.</param>
        /// <param name="type">The type of the object.</param>
        public static void WriteType(this JsonWriter writer, Type type)
        {
            writer.WritePropertyName("$type");
            writer.WriteValue(type.GetJsonTypeString());
        }
        
        /// <summary>
        /// Gets the JSON type string representation of the specified type.
        /// </summary>
        /// <param name="type">The type to get the JSON type string for.</param>
        /// <returns>The JSON type string representation of the specified type.</returns>
        public static string GetJsonTypeString(this Type type)
        {
            var typeName = type.FullName;
            var assemblyName = type.Assembly.GetName().Name;
            
            return $"{typeName}, {assemblyName}";
        }

        #endregion
    }
}