using NoReleaseDate.Variables.SaveDataVariable.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NoReleaseDate.Variables.SaveDataVariable.Editor
{
    [CustomPropertyDrawer(typeof(SaveData))]
    public class SaveDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Główny kontener poziomy
            var horizontalContainer = new VisualElement();
            horizontalContainer.style.flexDirection = FlexDirection.Row;
            horizontalContainer.style.alignItems = Align.Center;
            horizontalContainer.style.flexGrow = 1;
            horizontalContainer.style.marginBottom = 2;

            // Etykieta z nazwą zmiennej
            var label = new Label(property.displayName);
            label.style.minWidth = 150;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.marginRight = 5;
            horizontalContainer.Add(label);

            // Przycisk "Show"
            var showButton = new Button(() =>
            {
                // Pobieramy obiekt SaveData
                var targetObject = property.serializedObject.targetObject;
                var saveData = fieldInfo.GetValue(targetObject) as SaveData;

                // Sprawdzamy, czy obiekt nie jest pusty
                if (saveData != null)
                {
                    // Otwieramy niestandardowe okno z tekstem JSON
                    SaveDataViewerWindow.ShowWindow(saveData.ToJson());
                }
            })
            {
                text = "Show"
            };

            // Stylizacja przycisku
            showButton.style.height = 20;
            showButton.style.flexGrow = 1;
            showButton.style.marginLeft = 5;
            showButton.style.marginTop = 2;
            showButton.style.marginBottom = 2;
            showButton.style.unityTextAlign = TextAnchor.MiddleCenter;

            // Dodajemy przycisk do kontenera
            horizontalContainer.Add(showButton);

            return horizontalContainer;
        }
    }
}