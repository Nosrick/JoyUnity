using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class LightLevelHelper
    {
        private static float Normalise(int light)
        {
            return light == 0 ? 1 : light / (float)GlobalConstants.MAX_LIGHT;
        }

        public static Color GetColour(int light, int minValue, int maxValue)
        {
            Color colour;
            if (light >= minValue && light <= maxValue)
            {
                colour = Color.clear;
            }
            else if (light > maxValue)
            {
                float alpha = Normalise(light);
                float hue = Normalise(light);
                colour = new Color(hue, hue, hue, alpha);
            }
            else
            {
                float alpha = Normalise(GlobalConstants.MAX_LIGHT - light);
                float hue = 1.0f - alpha;
                colour = new Color(hue, hue, hue, alpha);
            }
            
            return colour;
        }
    }
}
