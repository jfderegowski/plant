﻿using TMPro;
using UnityEngine;

namespace fefek5.Systems.ThemeSystem.Runtime
{
    [AddComponentMenu("No Release Date/Theme System/Theme Controller Text")]
    [RequireComponent(typeof(TMP_Text))]
    public class ThemeControllerText : ThemeController
    {
        public ColorType colorType;

        private TMP_Text _text;
        
        protected override void Awake()
        {
            _text = GetComponent<TMP_Text>();
            
            base.Awake();
        }

        protected override void UpdateTheme()
        {
            if (!ThemeManager || !_text) return;
            
            if (_text.color == ThemeManager.Current.GetColor(colorType)) return;
            
            _text.color = ThemeManager.Current.GetColor(colorType);
        }
    }
}