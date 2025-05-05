using fefek5.Common.Runtime.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace fefek5.Common.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(LabelAttribute), true)]
    public class LabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelAttribute = (LabelAttribute)attribute;

            label.text = labelAttribute.Label;

            EditorGUI.PropertyField(position, property, label);
        }
    }
}