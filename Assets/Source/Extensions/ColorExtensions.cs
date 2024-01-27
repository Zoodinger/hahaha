using UnityEngine;

namespace Hahaha.Extensions {
    public static class ColorExtensions {
        public static Color WithAlpha(this Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color WithAlphaMultiplied(this Color color, float alpha) {
            return new Color(color.r, color.g, color.b, color.a * alpha);
        }
    }
}
