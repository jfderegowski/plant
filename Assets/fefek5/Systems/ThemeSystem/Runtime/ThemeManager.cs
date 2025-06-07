using fefek5.Common.Runtime.Helpers;
using fefek5.Systems.SingletonSystem.Runtime;
using UnityEngine;

namespace fefek5.Systems.ThemeSystem.Runtime
{
    [CreateAssetMenu(fileName = "ThemeManager", menuName = MenuPaths.fefek5.Systems.ThemeSystem.PATH + "/Theme Manager")]
    public class ThemeManager : SingletonObject<ThemeManager>
    {
        public ThemeColorPalette Current
        {
            get
            {
#if UNITY_EDITOR
                if (DefaultColorPalette)
                    return DefaultColorPalette;

                var newColorPalette = CreateInstance<ThemeColorPalette>();
                    
                UnityEditor.AssetDatabase.CreateAsset(newColorPalette,
                    "Assets/Resources/DefaultThemeColorPalette.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
                    
                DefaultColorPalette = newColorPalette;
#endif
                return DefaultColorPalette;
            }
        }

        public ThemeColorPalette DefaultColorPalette;
    }
}
