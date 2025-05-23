﻿using fefek5.Common.Runtime.Helpers;
using fefek5.Systems.ThemeSystem.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace fefek5.Systems.ThemeSystem.Editor
{
    public static class ThemeManagerEditor
    {
        /// <summary>
        /// Open the SingletonsCollection in the inspector
        /// </summary>
        [MenuItem(MenuPaths.fefek5.Systems.ThemeSystem.PATH + "/Open Theme Manager")]
        private static void OpenThemeManager() => EditorUtility.OpenPropertyEditor(ThemeManager.Instance);
        
        /// <summary>
        /// Add a ThemeControllerImage to the Image component
        /// </summary>
        /// <param name="command"></param>
        [MenuItem(MenuPaths.CONTEXT.Image.PATH + "/Add Theme Controller")]
        private static void AddUIThemeController(MenuCommand command)
        {
            var image = (Image)command.context;
            
            if (!image.gameObject.GetComponent<ThemeControllerImage>()) 
                image.gameObject.AddComponent<ThemeControllerImage>();
        }
        
        /// <summary>
        /// Add a ThemeControllerText to the TextMeshProUGUI component
        /// </summary>
        /// <param name="command"></param>
        [MenuItem(MenuPaths.CONTEXT.TMP_Text.PATH + "/Add Theme Controller")]
        private static void AddTMPThemeController(MenuCommand command)
        {
            var text = (TextMeshProUGUI)command.context;
            
            if (!text.gameObject.GetComponent<ThemeControllerText>()) 
                text.gameObject.AddComponent<ThemeControllerText>();
        }
    }
}