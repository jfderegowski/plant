﻿using fefek5.Common.Runtime.Extensions;
using fefek5.Common.Runtime.Helpers;
using UnityEngine;

namespace fefek5.Systems.ThemeSystem.Runtime
{
    /// <summary>
    /// Color Palette for the UI Theme System
    /// </summary>
    [CreateAssetMenu(fileName = "ColorPalette", menuName = MenuPaths.fefek5.Systems.ThemeSystem.PATH + "/Color Palette")]
    public class ThemeColorPalette : ScriptableObject
    {
        public Color firstColor = new Color().FromHex("#fffef5");
        public Color secondColor = new Color().FromHex("#313c50");
        public Color thirdColor = new Color().FromHex("#475866");
        public Color fourthColor = new Color().FromHex("#ad5143");

        public Color GetColor(ColorType colorType) =>
            colorType switch {
                ColorType.First => firstColor,
                ColorType.Second => secondColor,
                ColorType.Third => thirdColor,
                ColorType.Fourth => fourthColor,
                _ => Color.white
            };
    }
}