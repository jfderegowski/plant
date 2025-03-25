using UnityEngine;

namespace fefek5.Common.Runtime.PropertyAttributes
{
    public class LabelAttribute : PropertyAttribute
    {
        public readonly string Label;

        public LabelAttribute(string label) => Label = label;
    }
}
