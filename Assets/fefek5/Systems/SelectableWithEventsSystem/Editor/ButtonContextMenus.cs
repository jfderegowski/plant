using fefek5.Systems.SelectableWithEventsSystem.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace fefek5.Systems.SelectableWithEventsSystem.Editor
{
    public static class ButtonContextMenus
    {
        [MenuItem("CONTEXT/Button/Change to EventButton")]
        public static void ChangeToEventButton(MenuCommand menuCommand)
        {
            var button = (Button)menuCommand.context; 
            
            var gameObject = button.gameObject;

            Object.DestroyImmediate(button);
            
            gameObject.AddComponent<ButtonWithEvents>();
        }
    }
}