using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace fefek5.Common.Editor.MenuItems
{
    public static class TransformMenuItems
    {
        #region Component Sorting

        [MenuItem("CONTEXT/Transform/Sort Components")]
        private static void SortComponents(MenuCommand command)
        {
            var transform = (Transform)command.context;
            var gameObject = transform.gameObject;

            // Get all components except Transform
            var components = gameObject.GetComponents<Component>()
                .Where(c => c is not Transform).ToArray();

            // Sort components by namespace, then inheritance depth, and then by name
            var sortedComponents = components.OrderBy(c => GetNamespacePriority(c.GetType()))
                .ThenBy(c => c.GetType().Namespace)
                .ThenByDescending(c => GetInheritanceDepth(c.GetType()))
                .ThenBy(c => c.GetType().Name)
                .ToArray();

            // Reorder components
            for (var i = 0; i < sortedComponents.Length; i++)
            {
                var targetIndex = i + 1; // Adjust for Transform being at index 0
                MoveComponentToIndex(sortedComponents[i], targetIndex);
            }

            Debug.Log($"Sorted {sortedComponents.Length.ToString()} components.");
        }

        private static int GetNamespacePriority(Type type) =>
            type.Namespace != null && type.Namespace.StartsWith("Unity") ? 0 : 1;

        private static int GetInheritanceDepth(Type type)
        {
            var depth = 0;
            while (type.BaseType != null)
            {
                depth++;
                type = type.BaseType;
            }

            return depth;
        }

        private static void MoveComponentToIndex(Component component, int targetIndex)
        {
            var currentIndex = Array.IndexOf(component.gameObject.GetComponents<Component>(), component);
            while (currentIndex > targetIndex)
            {
                ComponentUtility.MoveComponentUp(component);
                currentIndex--;
            }

            while (currentIndex < targetIndex)
            {
                ComponentUtility.MoveComponentDown(component);
                currentIndex++;
            }
        }

        #endregion

        #region Normalize Scale

        [MenuItem("CONTEXT/Transform/Restore Scale")]
        private static void NormalizeScaleAndResetChildScales(MenuCommand command)
        {
            var transform = (Transform)command.context;
            var childList = transform.GetComponentsInChildren<Transform>(true);

            var positions = childList.ToDictionary(child => child, child => child.position);

            // Start an undo group
            const string undoGroupName = "Restore Scale";
            Undo.SetCurrentGroupName(undoGroupName);
            var undoGroup = Undo.GetCurrentGroup();
            
            // Record the root object
            Undo.RegisterCompleteObjectUndo(transform, undoGroupName);
            
            transform.localScale = Vector3.one;
            foreach (var child in childList)
            {
                // Record each child transform
                Undo.RegisterCompleteObjectUndo(child, undoGroupName);
                
                child.localScale = Vector3.one;
                child.position = positions[child];
            }
            
            // Close the undo group
            Undo.CollapseUndoOperations(undoGroup);
            
            // Mark the scene as dirty
            EditorUtility.SetDirty(transform);
        }

        #endregion

        #region Rounding
        
        [MenuItem("CONTEXT/Transform/Round to 1")]
        private static void RoundTransformTo1(MenuCommand command)
        {
            var transform = command.context as Transform;
            if (transform == null) return;

            RoundTransform(transform, 1);
        }
        
        [MenuItem("CONTEXT/Transform/Round to 2")]
        private static void RoundTransformTo2(MenuCommand command)
        {
            var transform = command.context as Transform;
            if (transform == null) return;

            RoundTransform(transform, 2);
        }
        
        [MenuItem("CONTEXT/Transform/Round to 3")]
        private static void RoundTransformTo3(MenuCommand command)
        {
            var transform = command.context as Transform;
            if (transform == null) return;

            RoundTransform(transform, 3);
        }
        
        [MenuItem("CONTEXT/Transform/Round to 4")]
        private static void RoundTransformTo4(MenuCommand command)
        {
            var transform = command.context as Transform;
            if (transform == null) return;

            RoundTransform(transform, 4);
        }

        private static void RoundTransform(Transform transform, int decimalPlaces)
        {
            var multiplier = Mathf.Pow(10, decimalPlaces);

            // Record the transform for undo
            Undo.RecordObject(transform, "Round Transform Values");

            // Round position
            transform.localPosition = new Vector3(
                Mathf.Round(transform.localPosition.x * multiplier) / multiplier,
                Mathf.Round(transform.localPosition.y * multiplier) / multiplier,
                Mathf.Round(transform.localPosition.z * multiplier) / multiplier
            );

            // Round rotation (Euler angles)
            var roundedEulerAngles = new Vector3(
                Mathf.Round(transform.localEulerAngles.x * multiplier) / multiplier,
                Mathf.Round(transform.localEulerAngles.y * multiplier) / multiplier,
                Mathf.Round(transform.localEulerAngles.z * multiplier) / multiplier
            );
            transform.localRotation = Quaternion.Euler(roundedEulerAngles);

            // Round scale
            transform.localScale = new Vector3(
                Mathf.Round(transform.localScale.x * multiplier) / multiplier,
                Mathf.Round(transform.localScale.y * multiplier) / multiplier,
                Mathf.Round(transform.localScale.z * multiplier) / multiplier
            );

            // Mark the scene as dirty
            EditorUtility.SetDirty(transform);
        }

        #endregion
    }
}