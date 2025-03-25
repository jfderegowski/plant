using fefek5.Variables.SaveDataVariable.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace fefek5.Variables.SaveDataVariable.Editor
{
    [CustomPropertyDrawer(typeof(SaveData))]
    public class SaveDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Główny kontener poziomy
            var container = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    flexGrow = 1,
                    height = 24,
                    marginBottom = 2
                }
            };

            // Etykieta z nazwą właściwości
            var label = new Label(property.displayName) {
                style = {
                    minWidth = 150,
                    width = 150,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginRight = 5
                }
            };

            // Przycisk do wyświetlania danych
            var showButton = new Button(() => {
                property.serializedObject.Update();
                var saveData = fieldInfo.GetValue(property.serializedObject.targetObject) as SaveData;
                if (saveData != null) 
                    SaveDataViewerWindow.ShowWindow(saveData.ToJson());
            }) {
                text = "Show",
                style = {
                    height = 20,
                    minWidth = 60,
                    flexGrow = 1,
                    marginLeft = 5,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };

            // Funkcja aktualizująca stan przycisku
            void UpdateButtonState()
            {
                property.serializedObject.Update();
                var saveData = fieldInfo.GetValue(property.serializedObject.targetObject) as SaveData;
                showButton.SetEnabled(saveData != null);
            }

            // Inicjalna aktualizacja stanu
            UpdateButtonState();

            // Automatyczne śledzenie zmian właściwości
            container.TrackPropertyValue(property, _ => UpdateButtonState());

            // Składanie UI
            container.Add(label);
            container.Add(showButton);

            return container;
        }
    }
}