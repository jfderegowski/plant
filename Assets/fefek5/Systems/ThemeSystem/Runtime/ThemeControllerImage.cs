﻿using UnityEngine;
using UnityEngine.UI;

namespace fefek5.Systems.ThemeSystem.Runtime
{
    [AddComponentMenu("No Release Date/Theme System/Theme Controller Image")]
    [RequireComponent(typeof(Image))]
    public class ThemeControllerImage : ThemeController
    {
        public ColorType colorType;

        private Image _image;

        protected override void Awake()
        {
            _image = GetComponent<Image>();
            
            base.Awake();
        }

        protected override void UpdateTheme()
        {
            if (!themeManager || !_image) return;
            
            if (_image.color == themeManager.Current.GetColor(colorType)) return;
            
            _image.color = themeManager.Current.GetColor(colorType);
        }
    }
}