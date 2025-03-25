using fefek5.Systems.SingletonSystem.Runtime;
using UnityEditor;

namespace fefek5.Systems.SingletonSystem.Editor
{
    public static class SingletonsStorageEditor
    {
        /// <summary>
        /// Open the SingletonsCollection in the inspector
        /// </summary>
        [MenuItem("No Release Date/Singleton System/Open Singletons Storage")]
        private static void OpenBindingsCollection() => EditorUtility.OpenPropertyEditor(SingletonsStorage.Instance);
    }
}