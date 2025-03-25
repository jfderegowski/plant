using fefek5.Systems.SingletonSystem.Runtime;
using UnityEngine;

namespace fefek5.Systems.ThemeSystem.Runtime
{
    [CreateAssetMenu(fileName = "ThemeManager", menuName = "No Release Date/Theme System/Theme Manager")]
    public class ThemeManager : SingletonObject<ThemeManager>
    {
        public ThemeColorPalette Current
        {
            get
            {
#if UNITY_EDITOR
                if (defaultColorPalette)
                    return defaultColorPalette;

                var newColorPalette = CreateInstance<ThemeColorPalette>();
                    
                UnityEditor.AssetDatabase.CreateAsset(newColorPalette,
                    "Assets/Resources/DefaultThemeColorPalette.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
                    
                defaultColorPalette = newColorPalette;
#endif
                return defaultColorPalette;
            }
        }

        public ThemeColorPalette defaultColorPalette;
    }
}
