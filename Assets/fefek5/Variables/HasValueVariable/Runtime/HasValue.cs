using System;
using UnityEngine;

namespace fefek5.Variables.HasValueVariable.Runtime
{
    [Serializable]
    public class HasValue<T>
    {
        public T value;
        public bool hasValue;

        public HasValue(T value, bool hasValue = false)
        {
            this.value = value;
            this.hasValue = hasValue;
        }

        public static implicit operator T(HasValue<T> hasValue)
        {
            if (!hasValue.hasValue)
                Debug.LogWarning("You using the value when the 'hasValue' is set to false");

            return hasValue.value;
        }
    }
}