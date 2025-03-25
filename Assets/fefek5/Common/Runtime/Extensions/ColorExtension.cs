using UnityEngine;

namespace fefek5.Common.Runtime.Extensions
{
    public static class ColorExtension
    {
        

        public static Color FromHex(this Color color, string hex) =>
            ColorUtility.TryParseHtmlString(hex, out var newCol) ? newCol : color;
    }
}