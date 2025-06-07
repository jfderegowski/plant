using UnityEngine;

namespace fefek5.Systems.ThemeSystem.Runtime
{
    [ExecuteInEditMode]
    public abstract class ThemeController : MonoBehaviour
    {
        public static ThemeManager ThemeManager => ThemeManager.Instance;

        protected virtual void Awake()
        {
            UpdateTheme();

            if (!Application.isPlaying) return;
            enabled = false;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                enabled = false;
                return;
            }
            
            UpdateTheme();
        }
#endif

        protected abstract void UpdateTheme();
    }
}